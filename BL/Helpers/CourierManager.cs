using BO;
using DalApi;
using DO;
using System.Security.Cryptography;
using System.Text;
//using DO = DO;
//using BO = BO;

namespace Helpers;


internal static class CourierManager
{
    internal static ObserverManager Observers = new();

    private static readonly IDal s_dal = Factory.Get;

    internal static BO.UserRole Login(int id, string password)
    {
        if (password is null)
            throw new BO.BlDoesNotExistException("Password cannot be null or empty");


        if (AdminManager.GetConfig().ManagerId == id && AdminManager.GetConfig().ManagerPassword == password)//צריך להוסיף בדיקת הצפנה
        {
            return UserRole.Admin;
        }
        var courier = s_dal.Courier.Read(id);//או לשנות ולהוסיף try catch
        if (courier == null)
            throw new BO.BlDoesNotExistException($"Courier with ID {id} does not exist.");
        string hashed = HashPassword(password);
        if (courier.Password == hashed)
        {
            return UserRole.Courier;
        }
        throw new BO.BlDoesNotExistException("Incorrect password.");
    }

    internal static IEnumerable<BO.CourierInList> GetCouriers(int requesterId , bool? onlyActive = null, BO.CourierListOrderBy? orderBy = null)
    {
        Tools.AuthorizeAdmin(requesterId);
        var couriers = s_dal.Courier.ReadAll();
        if (onlyActive is not null)
            couriers = couriers.Where(c => c.IsActive == onlyActive.Value);
        var newListOfCouriers = couriers
            .Select(c => CourierFromDoToBo(c))
            .Select(bo => new BO.CourierInList
            {
                Id = bo.Id,
                FullName = bo.FullName,
                PhoneNumber = bo.PhoneNumber,
                IsActive = bo.IsActive,
                EmploymentStartDate = bo.EmploymentStartDate,
                DeliveryType = bo.DeliveryType,
                NumberOfDeliveriesCourierCompletedOnTime = bo.NumberOfDeliveriesCompletedOnTime,
                NumberOfDeliveriesCourierCompletedLate = bo.NumberOfDeliveriesCompletedLate,
                CurrentOrderInProgress = bo.ordersInProgress
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
        return CourierFromDoToBo(courier);
    }

    internal static void UpdateCourier(int requesterId , BO.Courier courier)
    {
        if (!Tools.IsAdmin(requesterId) && requesterId != courier.Id)
            throw new BO.BlUnauthorizedAccessException("Not allowed");

        var existingCourier = s_dal.Courier.Read(courier.Id);
        if (existingCourier == null)
            throw new BO.BlDoesNotExistException($"Courier with ID {courier.Id} does not exist.");
        ValidateCourierBasics(courier, forCreate: false);

        string finalPassword = ProcessPasswordOnUpdate(courier.Password, existingCourier.Password);

        var updatedCourier = new DO.Courier
        {
            Id = courier.Id,
            FullName = courier.FullName,
            PhoneNumber = courier.PhoneNumber,
            Gmail = courier.Gmail,
            Password = finalPassword,
            IsActive = courier.IsActive,
            DeliveryType = (DO.DeliveryType)courier.DeliveryType,
            MaxPersonalDeliveryDistance = courier.MaxPersonalDeliveryDistance,
            EmploymentStartDate = courier.EmploymentStartDate
        };
        s_dal.Courier.Update(updatedCourier);

        Observers.NotifyItemUpdated(courier.Id);
        Observers.NotifyListUpdated();
    }

    internal static void CreateCourier(int requesterId , BO.Courier courier)
    {
        Tools.AuthorizeAdmin(requesterId);
        ValidateCourierBasics(courier,forCreate: true);
        string hashedPassword = ProcessPasswordOnCreate(courier.Password);
        var newCourier = new DO.Courier
        {
            Id = courier.Id,
            FullName = courier.FullName,
            PhoneNumber = courier.PhoneNumber,
            Gmail = courier.Gmail,
            Password = hashedPassword,
            IsActive = courier.IsActive,
            DeliveryType = (DO.DeliveryType)courier.DeliveryType,
            MaxPersonalDeliveryDistance = courier.MaxPersonalDeliveryDistance,
            EmploymentStartDate = courier.EmploymentStartDate
        };
        s_dal.Courier.Create(newCourier);

        Observers.NotifyListUpdated();
    }

    internal static void DeleteCourier(int requesterId, int courierId)
    {
        Tools.AuthorizeAdmin(requesterId);
        var existingCourier = s_dal.Courier.Read(courierId);
        if (existingCourier == null)
            throw new BO.BlDoesNotExistException($"Courier with ID {courierId} does not exist.");
        if (s_dal.Delivery.ReadAll().Any(d => d.CourierId == courierId))
            throw new BO.BlInvalidOperationException("Cannot delete courier with active deliveries.");
        s_dal.Courier.Delete(courierId);

        Observers.NotifyListUpdated();
        Observers.NotifyItemUpdated(courierId);
    }

    // Help functions

    private static BO.Courier CourierFromDoToBo(DO.Courier d) 
    {
        var deliverys = s_dal.Delivery.ReadAll().Where(del => del.CourierId == d.Id);

        var activeDelivery = deliverys
            .Where(del => del.TypeOfDeliveryCompletionTime is null)
            .OrderByDescending(del => del.DeliveryStartTime)
            .FirstOrDefault();
        BO.OrderInProgress? newOrderInProgress = null;
        if (activeDelivery is not null)
        {
            var order = s_dal.Order.Read(activeDelivery.OrderId);
            if(order is not null)
            {
                newOrderInProgress = OrderManager.AddOrderInProgress(order, activeDelivery);
            }
        }
        int onTime = 0, late = 0;
        onTime = deliverys.Count(del => GetScheduleStatusOfDelivery(del) == ScheduleStatus.OnTime);
        late = deliverys.Count(del => GetScheduleStatusOfDelivery(del) == ScheduleStatus.Late);
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
            c.EmploymentStartDate = AdminManager.Now;//לטפל בהרשאה לשנות
    }

    private static bool IsValidPhone(string phone) =>
            !string.IsNullOrWhiteSpace(phone) && phone.Length == 10 && phone.StartsWith("0") && phone.All(char.IsDigit);

    private static bool IsValidEmail(string email) =>
        !string.IsNullOrWhiteSpace(email) && email.Contains("@") && email.Contains(".");
    
    private static void AuthorizeAdminOrThatCourier(int requesterId, int courierId)
    {
        if (requesterId == courierId) return;
        Tools.AuthorizeAdmin(requesterId);
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

    internal static BO.ScheduleStatus GetScheduleStatusOfDelivery(DO.Delivery delivery)
    {
        if (delivery.DeliveryCompletionTime != null)
        {
            var order = s_dal.Order.Read(delivery.OrderId);
            if (order != null)
            {
                DateTime maxDeliveryTime = order.OrderPlacementTime + s_dal.Config.MaxTimeRangeForDelivery;
                
                if (delivery.DeliveryCompletionTime <= maxDeliveryTime)
                    return BO.ScheduleStatus.OnTime;
                else
                    return BO.ScheduleStatus.Late;
            }
        }
        
        DateTime expectedDeliveryTime = delivery.DeliveryStartTime + OrderManager.ExpectedDeliveryTime(delivery);
        
        var order2 = s_dal.Order.Read(delivery.OrderId);
        if (order2 != null)
        {
            DateTime maxDeliveryTime = order2.OrderPlacementTime + s_dal.Config.MaxTimeRangeForDelivery;
            DateTime riskThresholdTime = maxDeliveryTime - s_dal.Config.RiskRange;
            if (AdminManager.Now > expectedDeliveryTime)
            {
                if (AdminManager.Now > maxDeliveryTime)
                    return BO.ScheduleStatus.Late;
                
                if (AdminManager.Now > riskThresholdTime)
                    return BO.ScheduleStatus.InRisk;
            }
        }

        return BO.ScheduleStatus.OnTime;
    }

    internal static BO.OrderStatus GetDeliveryStatus(DO.Delivery delivery)
    {
        if (delivery.DeliveryCompletionTime is not null)
        {
            return delivery.TypeOfDeliveryCompletionTime switch
            {
                DO.TypeOfDeliveryCompletionTime.Supplied => BO.OrderStatus.Delivered,
                DO.TypeOfDeliveryCompletionTime.RefusedByCustomer => BO.OrderStatus.DeliveryDeclinedByCustomer,
                DO.TypeOfDeliveryCompletionTime.Canceled => BO.OrderStatus.Canceled,
                DO.TypeOfDeliveryCompletionTime.Failed => BO.OrderStatus.Canceled,
                DO.TypeOfDeliveryCompletionTime.CustomerNotFound => BO.OrderStatus.Canceled,
                _ => BO.OrderStatus.InProgress
            };
        }
        
        if (delivery.CourierId == 0)
            return BO.OrderStatus.Open;
        
        return BO.OrderStatus.InProgress;
    }

    private static bool IsStrongPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        bool hasUpper = password.Any(char.IsUpper);
        bool hasLower = password.Any(char.IsLower);
        bool hasDigit = password.Any(char.IsDigit);
        bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

        return password.Length >= 8 && hasUpper && hasLower && hasDigit && hasSpecial;
    }

    private static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(password);
        byte[] hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash); // מחרוזת הקסדצימלית
    }

    private static string ProcessPasswordOnCreate(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new BO.BlDataValidationException("Password is required");

        if (!IsStrongPassword(password))
            throw new BO.BlDataValidationException("Password is not strong enough");

        return HashPassword(password);
    }
    private static string ProcessPasswordOnUpdate(string? newPassword, string existingHashedPassword)
    {
        // אם המשתמש לא שינה סיסמה, משאירים את הקיימת
        if (string.IsNullOrWhiteSpace(newPassword))
            return existingHashedPassword;

        if (!IsStrongPassword(newPassword))
            throw new BO.BlDataValidationException("Password is not strong enough");

        return HashPassword(newPassword);
    }
}




