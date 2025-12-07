using BO;
using DalApi;
using DO;
using System.Security.Cryptography;
using System.Text;
//using DO = DO;
//using BO = BO;

namespace Helpers;

/// <summary>
/// Manages courier-related operations including authentication, retrieval, creation, updates, and deletions.
/// Also handles delivery status calculations and observer notifications.
/// </summary>
/// <remarks>
/// This static class provides all business logic for courier management, including password hashing,
/// validation, and transformation between data and business layer objects. It also manages
/// observer notifications for list and item updates.
/// </remarks>
internal static class CourierManager
{
    /// <summary>
    /// Gets the observer manager for notifying subscribers of courier list and item changes.
    /// </summary>
    internal static ObserverManager Observers = new();

    /// <summary>
    /// Gets the Data Access Layer (DAL) factory instance for accessing data operations.
    /// </summary>
    private static readonly IDal s_dal = Factory.Get;

    /// <summary>
    /// Authenticates a user by ID and password, returning their role.
    /// </summary>
    /// <param name="id">The user's ID to authenticate.</param>
    /// <param name="password">The user's password to verify.</param>
    /// <returns>A UserRole value indicating whether the user is an Admin or Courier.</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when password is null, courier doesn't exist, or password is incorrect.</exception>
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
        if (courier.Password == hashed || courier.Password == password)
        {
            return UserRole.Courier;
        }
        throw new BO.BlDoesNotExistException("Incorrect password.");
    }

    /// <summary>
    /// Retrieves a list of couriers with optional filtering and sorting.
    /// </summary>
    /// <param name="requesterId">The ID of the user requesting the list (must be admin).</param>
    /// <param name="onlyActive">Optional filter to show only active couriers. Null shows all.</param>
    /// <param name="orderBy">Optional sorting criteria for the returned list.</param>
    /// <returns>An enumerable collection of CourierInList objects matching the criteria.</returns>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the requester is not an admin.</exception>
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

    /// <summary>
    /// Retrieves a specific courier by ID.
    /// </summary>
    /// <param name="requesterId">The ID of the user requesting the courier (admin or the courier themselves).</param>
    /// <param name="courierId">The ID of the courier to retrieve.</param>
    /// <returns>A Courier object containing the requested courier's information.</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the courier doesn't exist.</exception>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the requester is not authorized to view this courier.</exception>
    internal static BO.Courier GetCourier(int requesterId, int courierId)
    {
        AuthorizeAdminOrThatCourier(requesterId, courierId);
        var courier = s_dal.Courier.Read(courierId);
        if (courier == null)
            throw new BO.BlDoesNotExistException($"Courier with ID {courierId} does not exist.");
        return CourierFromDoToBo(courier);
    }

    /// <summary>
    /// Updates an existing courier's information.
    /// </summary>
    /// <param name="requesterId">The ID of the user requesting the update (admin or the courier themselves).</param>
    /// <param name="courier">The Courier object containing updated information.</param>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the requester is not authorized to update this courier.</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the courier doesn't exist.</exception>
    /// <exception cref="BO.BlDataValidationException">Thrown if validation fails.</exception>
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

    /// <summary>
    /// Creates a new courier in the system.
    /// </summary>
    /// <param name="requesterId">The ID of the user creating the courier (must be admin).</param>
    /// <param name="courier">The Courier object containing the new courier's information.</param>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the requester is not an admin.</exception>
    /// <exception cref="BO.BlDataValidationException">Thrown if validation fails.</exception>
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

    /// <summary>
    /// Deletes a courier from the system.
    /// </summary>
    /// <param name="requesterId">The ID of the user deleting the courier (must be admin).</param>
    /// <param name="courierId">The ID of the courier to delete.</param>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the requester is not an admin.</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the courier doesn't exist.</exception>
    /// <exception cref="BO.BlInvalidOperationException">Thrown if the courier has active deliveries.</exception>
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

    /// <summary>
    /// Converts a Data Access Layer Courier object to a Business Logic Courier object.
    /// </summary>
    /// <param name="d">The DO.Courier object to convert.</param>
    /// <returns>A BO.Courier object with calculated metrics and current delivery status.</returns>
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

    /// <summary>
    /// Validates basic courier information.
    /// </summary>
    /// <param name="c">The courier object to validate.</param>
    /// <param name="forCreate">Whether this is a validation for creation (true) or update (false).</param>
    /// <exception cref="BO.BlDataValidationException">Thrown if any validation fails.</exception>
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

    /// <summary>
    /// Validates if a phone number meets the required format.
    /// </summary>
    /// <param name="phone">The phone number to validate.</param>
    /// <returns>True if the phone is valid (10 digits starting with 0); otherwise, false.</returns>
    private static bool IsValidPhone(string phone) =>
            !string.IsNullOrWhiteSpace(phone) && phone.Length == 10 && phone.StartsWith("0") && phone.All(char.IsDigit);

    /// <summary>
    /// Validates if an email address meets the required format.
    /// </summary>
    /// <param name="email">The email to validate.</param>
    /// <returns>True if the email is valid (contains @ and .); otherwise, false.</returns>
    private static bool IsValidEmail(string email) =>
        !string.IsNullOrWhiteSpace(email) && email.Contains("@") && email.Contains(".");
    
    /// <summary>
    /// Authorizes a request by checking if the requester is an admin or the target courier.
    /// </summary>
    /// <param name="requesterId">The ID of the user making the request.</param>
    /// <param name="courierId">The ID of the courier being accessed.</param>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the requester is not authorized.</exception>
    private static void AuthorizeAdminOrThatCourier(int requesterId, int courierId)
    {
        if (requesterId == courierId) return;
        Tools.AuthorizeAdmin(requesterId);
    }

    /// <summary>
    /// Sorts a collection of couriers based on the specified ordering criteria.
    /// </summary>
    /// <param name="couriers">The collection of couriers to sort.</param>
    /// <param name="orderBy">The sorting criteria to apply.</param>
    /// <returns>A sorted enumerable of couriers.</returns>
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

    /// <summary>
    /// Gets the average speed in kilometers per hour for a specific delivery type.
    /// </summary>
    /// <param name="deliveryType">The delivery type (Car, Motorcycle, Bicycle, OnFoot).</param>
    /// <returns>The average speed in km/h; 0 if unknown delivery type.</returns>
    internal static double GetAverageSpeedKmh(DO.DeliveryType deliveryType)
    {
        return deliveryType switch
        {
            DO.DeliveryType.Car => AdminManager.GetConfig().AvgSpeedKmhForCar,
            DO.DeliveryType.Motorcycle => AdminManager.GetConfig().AvgSpeedKmhForMotorcycle,
            DO.DeliveryType.Bicycle => AdminManager.GetConfig().AvgSpeedKmhForBicycle,
            DO.DeliveryType.OnFoot => AdminManager.GetConfig().AvgSpeedKmhForFoot,
            _ => 0.0
        };
    }

    /// <summary>
    /// Determines the scheduling status of a delivery (OnTime, Late, InRisk).
    /// </summary>
    /// <param name="delivery">The delivery to evaluate.</param>
    /// <returns>A ScheduleStatus value indicating the delivery's timing status.</returns>
    internal static BO.ScheduleStatus GetScheduleStatusOfDelivery(DO.Delivery delivery)
    {
        if (delivery.DeliveryCompletionTime != null)
        {
            var order = s_dal.Order.Read(delivery.OrderId);
            if (order != null)
            {
                DateTime maxDeliveryTime = order.OrderPlacementTime + AdminManager.GetConfig().MaxTimeRangeForDelivery;
                
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
            DateTime maxDeliveryTime = order2.OrderPlacementTime + AdminManager.GetConfig().MaxTimeRangeForDelivery;
            DateTime riskThresholdTime = maxDeliveryTime - AdminManager.GetConfig().RiskRange;
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

    /// <summary>
    /// Determines the operational status of a delivery based on its completion type.
    /// </summary>
    /// <param name="delivery">The delivery to evaluate.</param>
    /// <returns>An OrderStatus value indicating the delivery's operational status.</returns>
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

    /// <summary>
    /// Checks if a password meets strength requirements.
    /// </summary>
    /// <param name="password">The password to validate.</param>
    /// <returns>True if the password is strong enough; otherwise, false.</returns>
    /// <remarks>A strong password must be at least 8 characters and contain uppercase, lowercase, digit, and special characters.</remarks>
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

    /// <summary>
    /// Hashes a password using SHA256 encryption.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>A hexadecimal string representation of the hashed password.</returns>
    private static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(password);
        byte[] hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash); // מחרוזת הקסדצימלית
    }

    /// <summary>
    /// Processes and validates a password for a new courier account.
    /// </summary>
    /// <param name="password">The password to process.</param>
    /// <returns>The hashed password ready for storage.</returns>
    /// <exception cref="BO.BlDataValidationException">Thrown if password is empty or not strong enough.</exception>
    private static string ProcessPasswordOnCreate(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new BO.BlDataValidationException("Password is required");

        if (!IsStrongPassword(password))
            throw new BO.BlDataValidationException("Password is not strong enough");

        return HashPassword(password);
    }

    /// <summary>
    /// Processes and validates a password for a courier account update.
    /// </summary>
    /// <param name="newPassword">The new password to process, or null/empty to keep existing password.</param>
    /// <param name="existingHashedPassword">The currently stored hashed password.</param>
    /// <returns>Either the newly hashed password or the existing hashed password if no update is provided.</returns>
    /// <exception cref="BO.BlDataValidationException">Thrown if new password is not strong enough.</exception>
    /// <remarks>If newPassword is null or empty, the existing hashed password is returned unchanged.</remarks>
    private static string ProcessPasswordOnUpdate(string? newPassword, string existingHashedPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
            return existingHashedPassword;

        if (!IsStrongPassword(newPassword))
            throw new BO.BlDataValidationException("Password is not strong enough");

        return HashPassword(newPassword);
    }
}



]


