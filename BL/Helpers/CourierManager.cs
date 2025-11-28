using BO;
using DalApi;
using DO;
//using DO = DO;
//using BO = BO;

namespace Helpers;


internal static class CourierManager
{
    private static readonly IDal s_dal = Factory.Get;

    internal static BO.UserRole Login(int id, string password)
    {
        if (password is null)
            throw new BO.BlDoesNotExistException("Password cannot be null or empty");

        var courier = s_dal.Courier.Read(id);
        if (courier == null)
            throw new BO.BlDoesNotExistException($"Courier with ID {id} does not exist.");

        if (s_dal.Config.ManagerId == id && s_dal.Config.ManagerPassword == password)//צריך להוסיף בדיקת הצפנה
        {
            return UserRole.Admin;
        }
        if (courier.Password == password)
        {
            return UserRole.Courier;
        }
        throw new BO.BlDoesNotExistException("Incorrect password.");
    }

    internal static IEnumerable<BO.CourierInList> GetCouriers(int requesterId , bool? onlyActive = null, BO.CourierListOrderBy? orderBy = null)
    {
        AuthorizeAdmin(requesterId);
        var couriers = s_dal.Courier.ReadAll();
        if (onlyActive is not null)
            couriers = couriers.Where(c => c.IsActive == onlyActive.Value);
        var newListOfCouriers = couriers
            .Select(c => new BO.CourierInList
            {
                Id = c.Id,
                FullName = c.FullName,
                IsActive = c.IsActive,
                EmploymentStartDate = c.EmploymentStartDate,
                DeliveryType = (BO.DeliveryType)c.DeliveryType,
                NumberOfDeliveriesCourierCompletedOnTime = 0,//לבדוק איך עושים
                NumberOfDeliveriesCourierCompletedLate = 0,//לבדוק איך עושים
                CurrentOrderInProgress = null
            })
            .SortCouriers(orderBy);
        return newListOfCouriers;
    }

    internal static BO.Courier GetCourier(int requesterId, int courierId)
    {
        AuthorizeAdminOrThatCourier(requesterId, courierId);
        var courier = s_dal.Courier.Read(courierId);
        if (courier == null)
            throw new BO.BlDoesNotExistException($"Courier with ID {courierId} does not exist.");
        return FromDoToBo(courier);
    }

    internal static void UpdateCourier(int requesterId , BO.Courier courier)
    {
        if (!IsAdmin(requesterId) && requesterId != courier.Id)
            throw new BO.BlUnauthorizedAccessException("Not allowed");

        var existingCourier = s_dal.Courier.Read(courier.Id);
        if (existingCourier == null)
            throw new BO.BlDoesNotExistException($"Courier with ID {courier.Id} does not exist.");
        ValidateCourierBasics(courier, forCreate: false);

        var updatedCourier = new DO.Courier
        {
            Id = courier.Id,
            FullName = courier.FullName,
            PhoneNumber = courier.PhoneNumber,
            Gmail = courier.Gmail,
            Password = courier.Password,
            IsActive = courier.IsActive,
            DeliveryType = (DO.DeliveryType)courier.DeliveryType,
            MaxPersonalDeliveryDistance = courier.MaxPersonalDeliveryDistance,
            EmploymentStartDate = courier.EmploymentStartDate
        };
        s_dal.Courier.Update(updatedCourier);
    }

    internal static void CreateCourier(int requesterId , BO.Courier courier)
    {
        AuthorizeAdmin(requesterId);
        ValidateCourierBasics(courier,forCreate: true);
        var newCourier = new DO.Courier
        {
            Id = courier.Id,
            FullName = courier.FullName,
            PhoneNumber = courier.PhoneNumber,
            Gmail = courier.Gmail,
            Password = courier.Password,
            IsActive = courier.IsActive,
            DeliveryType = (DO.DeliveryType)courier.DeliveryType,
            MaxPersonalDeliveryDistance = courier.MaxPersonalDeliveryDistance,
            EmploymentStartDate = courier.EmploymentStartDate
        };
        s_dal.Courier.Create(newCourier);
    }

    internal static void DeleteCourier(int requesterId, int courierId)
    {
        AuthorizeAdmin(requesterId);
        var existingCourier = s_dal.Courier.Read(courierId);
        if (existingCourier == null)
            throw new BO.BlDoesNotExistException($"Courier with ID {courierId} does not exist.");
        if (s_dal.Delivery.ReadAll().Any(d => d.CourierId == courierId))
            throw new BO.BlInvalidOperationException("Cannot delete courier with active deliveries.");
        s_dal.Courier.Delete(courierId);
    }

    private static BO.Courier FromDoToBo(DO.Courier d) 
    {
        var deliverys = s_dal.Delivery.ReadAll().Where(del => del.CourierId == d.Id);

        var activeDelivery = deliverys
            .Where(del => del.TypeOfDeliveryCompletionTime is null)
            .OrderByDescending(del => del.DeliveryStartTime)
            .FirstOrDefault();
        BO.OrderInProgress? newOrderInProgress = null;

        if(activeDelivery is not null)
        {
            var order = s_dal.Order.Read(activeDelivery.OrderId);
            if(order is not null)
            {
                newOrderInProgress = new BO.OrderInProgress
                {
                    DeliveryId = activeDelivery.Id,
                    OrderId = order.Id,
                    OrderType = (BO.OrderType)order.OrderType,
                    DescriptionOfOrder = order.DescriptionOfOrder,
                    AddressOfOrder = order.AddressOfOrder,
                    AirDistance = AirDistance(order.Latitude, order.Longitude, s_dal.Config.Latitude.GetValueOrDefault(), s_dal.Config.Longitude.GetValueOrDefault()),
                    ActualDistance = activeDelivery.ActualDistance,
                    CustomerName = order.CustomerName,
                    CustomerPhone = order.CustomerPhone,
                    OrderPlacementTime = order.OrderPlacementTime,
                    PickUpTime = activeDelivery.DeliveryStartTime,
                    DeliveryTime = activeDelivery.DeliveryStartTime + ExpectedDeliveryTime(activeDelivery),
                    MaxDelivryTime = activeDelivery.DeliveryStartTime + s_dal.Config.MaxTimeRangeForDelivery,
                    OrderStatus = GetOrderStatus(activeDelivery),
                    ScheduleStatus = GetScheduleStatus(activeDelivery),
                    TimeLeftToDelivery = (activeDelivery.DeliveryStartTime + s_dal.Config.MaxTimeRangeForDelivery) - AdminManager.Now
                };
            }
        }
        int onTime = 0, late = 0;
        onTime = deliverys.Count(del => GetScheduleStatus(del) == ScheduleStatus.OnTime);
        late = deliverys.Count(del => GetScheduleStatus(del) == ScheduleStatus.Late);
        return new BO.Courier
        {
            Id = d.Id,
            FullName = d.FullName,
            PhoneNumber = d.PhoneNumber,
            Gmail = d.Gmail,
            Password = d.Password,
            IsActive = d.IsActive,
            DeliveryType = (BO.DeliveryType)d.DeliveryType,
            MaxPersonalDeliveryDistance = d.MaxPersonalDeliveryDistance,
            EmploymentStartDate = d.EmploymentStartDate,
            NumberOfDeliveriesCompletedOnTime = onTime,
            NumberOfDeliveriesCompletedLate = late,
            ordersInProgress = newOrderInProgress
        };
    }

    private static void ValidateCourierBasics(BO.Courier c, bool forCreate)
    {
        if (c.Id <= 0)
            throw new BO.BlDataValidationException("Invalid Id");
        if (string.IsNullOrWhiteSpace(c.FullName))
            throw new BO.BlDataValidationException("Name required");
        if (!IsValidPhone(c.PhoneNumber))
            throw new BO.BlDataValidationException("Phone must be 10 digits starting with 0");
        if (!IsValidEmail(c.Gmail))
            throw new BO.BlDataValidationException("Invalid email");
        if(forCreate && c.EmploymentStartDate == default)
            c.EmploymentStartDate = AdminManager.Now;
    }

    private static bool IsValidPhone(string phone) =>
            !string.IsNullOrWhiteSpace(phone) && phone.Length == 10 && phone.StartsWith("0") && phone.All(char.IsDigit);

    private static bool IsValidEmail(string email) =>
        !string.IsNullOrWhiteSpace(email) && email.Contains("@") && email.Contains(".");
    
    private static void AuthorizeAdmin(int requesterId)
    {
        var u = s_dal.Config.ManagerId;
        if (requesterId != s_dal.Config.ManagerId)
            throw new BO.BlUnauthorizedAccessException("Only admin users are authorized to perform this action.");
    }

    private static bool IsAdmin(int requesterId)
    {
        return requesterId == s_dal.Config.ManagerId;
    }

    private static void AuthorizeAdminOrThatCourier(int requesterId, int courierId)
    {
        if (requesterId == courierId) return;
        AuthorizeAdmin(requesterId);
    }

    private static IEnumerable<BO.CourierInList> SortCouriers(this IEnumerable<BO.CourierInList> couriers, BO.CourierListOrderBy? orderBy)
    {
        return orderBy switch
        {
            BO.CourierListOrderBy.ById => couriers.OrderBy(c => c.Id),
            BO.CourierListOrderBy.ByName => couriers.OrderBy(c => c.FullName),
            BO.CourierListOrderBy.ByActive => couriers.OrderBy(c => c.IsActive),
            BO.CourierListOrderBy.EmploymentStartDate => couriers.OrderBy(c => c.EmploymentStartDate),
            BO.CourierListOrderBy.DeliveryType => couriers.OrderBy(c => c.DeliveryType),
            _ => couriers
        };
    }//הסבר

    internal static TimeSpan ExpectedDeliveryTime(DO.Delivery delivery)
    {
        double averageSpeed = GetAverageSpeedKmh(delivery.DeliveryType);
        double distance = delivery.ActualDistance ?? 0;
        double hours = averageSpeed > 0 ? distance / averageSpeed : 0;
        return TimeSpan.FromHours(hours);
    }

    internal static TimeSpan TimeLeftToDelivery(DO.Delivery delivery , DO.Order order)
    {
        return s_dal.Config.MaxTimeRangeForDelivery;
    }

    internal static BO.OrderStatus GetOrderStatus(DO.Delivery delivery)
    {

        if (delivery.DeliveryCompletionTime is not null)
        {
            if (delivery.TypeOfDeliveryCompletionTime == DO.TypeOfDeliveryCompletionTime.Supplied)
                return BO.OrderStatus.Delivered;
            if (delivery.TypeOfDeliveryCompletionTime == DO.TypeOfDeliveryCompletionTime.RefusedByCustomer)
                return BO.OrderStatus.DeliveryDeclinedByCustomer;
            if (delivery.TypeOfDeliveryCompletionTime == DO.TypeOfDeliveryCompletionTime.Canceled)
                return BO.OrderStatus.Canceled;
        }
        else if (delivery.CourierId == 0)
            return BO.OrderStatus.Open;
        return BO.OrderStatus.InProgress;
    }

    internal static BO.ScheduleStatus GetScheduleStatus(DO.Delivery delivery)
    {
        if (delivery.DeliveryCompletionTime != null)
        {
            if (delivery.DeliveryCompletionTime <= delivery.DeliveryStartTime + s_dal.Config.MaxTimeRangeForDelivery)
                return BO.ScheduleStatus.OnTime;
            else
                return BO.ScheduleStatus.Late;
        }
        else
        {
            if (AdminManager.Now <= delivery.DeliveryStartTime + ExpectedDeliveryTime(delivery))
            {
                return BO.ScheduleStatus.OnTime;
            }
            else if (AdminManager.Now <= delivery.DeliveryStartTime + s_dal.Config.MaxTimeRangeForDelivery)
            {
                return BO.ScheduleStatus.InRisk;
            }
            else
            {
                return BO.ScheduleStatus.Late;
            }
        }
           

    }

    internal static double GetAverageSpeedKmh(DO.DeliveryType deliveryType)
    {
        return deliveryType switch
        {
            DO.DeliveryType.Car => s_dal.Config.AvgSpeedKmhForCar,
            DO.DeliveryType.Motorcycle => s_dal.Config.AvgSpeedKmhForMotorcycle,
            DO.DeliveryType.Bicycle => s_dal.Config.AvgSpeedKmhForBicycle,
            DO.DeliveryType.OnFoot => s_dal.Config.AvgSpeedKmhForFoot,
            _ => 0.0
        };
    }

    private const double EarthRadiusKm = 6371.0;
    internal static double AirDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double dLat = DegreesToRadians(lat2 - lat1);
        double dLon = DegreesToRadians(lon2 - lon1);

        lat1 = DegreesToRadians(lat1);
        lat2 = DegreesToRadians(lat2);

   
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusKm * c;
    }
    private static double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }

}

