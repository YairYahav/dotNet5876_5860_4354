using DalApi;
using DO;
using BO;
using System;

namespace Helpers;

/// <summary>
/// Manages order-related operations including creation, retrieval, updates, cancellations, and delivery assignments.
/// Also handles order status calculations and observer notifications.
/// </summary>
/// <remarks>
/// This static class provides comprehensive business logic for order management, including filtering,
/// sorting, validation, and transformation between data and business layer objects. It also manages
/// observer notifications for list and item updates and integrates with geocoding services for address handling.
/// </remarks>
internal static class OrderManager
{
    /// <summary>
    /// Gets the observer manager for notifying subscribers of order list and item changes.
    /// </summary>
    internal static ObserverManager Observers = new();

    /// <summary>
    /// Gets the Data Access Layer (DAL) factory instance for accessing data operations.
    /// </summary>
    private static readonly IDal s_dal = Factory.Get;

    /// <summary>
    /// Retrieves a summary of all orders organized by their status and scheduling status combination.
    /// </summary>
    /// <param name="requesterId">The ID of the user requesting the summary (must be admin).</param>
    /// <returns>An array where each index represents a combination of OrderStatus and ScheduleStatus, containing the count of orders in that state.</returns>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the requester is not an admin.</exception>
    /// <remarks>The array is organized as [orderStatusIndex * scheduleStatusCount + scheduleStatusIndex].</remarks>
    internal static int[] GetOrdersSummary(int requesterId)
    {
        Tools.AuthorizeAdmin(requesterId);
        //להוסיף try catch
        var orders = s_dal.Order.ReadAll();

        int orderStatusCount = Enum.GetValues(typeof(BO.OrderStatus)).Length;
        int scheduleStatusCount = Enum.GetValues(typeof(BO.ScheduleStatus)).Length;
        int totalCombinations = orderStatusCount * scheduleStatusCount;
        int[] summary = new int[totalCombinations];
        var grouped = orders
            .GroupBy(order => new
            {
                OrderStatus = GetOrderStatus(order),
                ScheduleStatus = GetScheduleStatusOfOrder(order)
            })
            .Select(g => new
            {
                OrderStatusIndex = (int)g.Key.OrderStatus,
                ScheduleStatusIndex = (int)g.Key.ScheduleStatus,
                Count = g.Count()
            })
            .ToArray();

        for (int i = 0; i < grouped.Length; i++)
        {
            int orderStatusIndex = grouped[i].OrderStatusIndex;
            int scheduleStatusIndex = grouped[i].ScheduleStatusIndex;

            int index = orderStatusIndex * scheduleStatusCount + scheduleStatusIndex;
            summary[index] = grouped[i].Count;
        }
        return summary;
    }

    /// <summary>
    /// Retrieves a filtered and sorted list of all orders.
    /// </summary>
    /// <param name="requesterId">The ID of the user requesting the list (must be admin).</param>
    /// <param name="filterBy">Optional filter criteria for the orders.</param>
    /// <param name="filterValue">The value to filter by (OrderStatus or ScheduleStatus depending on filterBy).</param>
    /// <param name="orderBy">Optional sorting criteria for the returned list.</param>
    /// <returns>An enumerable collection of OrderInList objects matching the criteria.</returns>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the requester is not an admin.</exception>
    internal static IEnumerable<BO.OrderInList> GetOrders(int requesterId,BO.OrderListFilterBy? filterBy = null,object? filterValue = null,BO.OrderListOrderBy? orderBy = null)
    {
        Tools.AuthorizeAdmin(requesterId);

        var orders = s_dal.Order.ReadAll();

        var projected = orders
            .Select(order => MapDoToOrderInList(order))
            .ToList();

        var filtered = (filterBy, filterValue) switch
        {
            (BO.OrderListFilterBy.ByStatus, BO.OrderStatus s) =>
                projected.Where(o => o.OrderStatus == s),

            (BO.OrderListFilterBy.ByTiming, BO.ScheduleStatus t) =>
                projected.Where(o => o.ScheduleStatus == t),

            _ => projected
        };

        var ordered = orderBy switch
        {
            BO.OrderListOrderBy.ByDistance =>
                filtered.OrderBy(o => o.AirDistance),

            BO.OrderListOrderBy.ByTiming =>
                filtered.OrderBy(o => o.ScheduleStatus),

            BO.OrderListOrderBy.ById =>
                filtered.OrderBy(o => o.OrderId),

            BO.OrderListOrderBy.ByStatus =>
                filtered.OrderBy(o => o.OrderStatus),

            _ => filtered.OrderBy(o => o.OrderStatus)
        };

        return ordered;
    }

    /// <summary>
    /// Retrieves a list of closed (completed) deliveries for a specific courier with optional filtering and sorting.
    /// </summary>
    /// <param name="requesterId">The ID of the user requesting the list (must be admin).</param>
    /// <param name="courierId">The ID of the courier whose closed deliveries to retrieve.</param>
    /// <param name="orderTypeFilter">Optional filter by order type.</param>
    /// <param name="orderBy">Optional sorting criteria for the returned list.</param>
    /// <returns>An enumerable collection of ClosedDeliveryInList objects for the specified courier.</returns>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the requester is not an admin.</exception>
    internal static IEnumerable<BO.ClosedDeliveryInList> GetClosedDeliveries(int requesterId,int courierId,BO.OrderType? orderTypeFilter = null,BO.ClosedOrdersListOrderBy? orderBy = null)
    {
        //להוסיף try catch
        Tools.AuthorizeAdmin(requesterId);
        var closedDeliveries = s_dal.Delivery.ReadAll()
            .Where(d => d.CourierId == courierId && d.DeliveryCompletionTime != null)
            .ToList();

        var projected = closedDeliveries
            .Select(MapDoToClosedDeliveryInList)
            .ToList();

        var filtered = orderTypeFilter is null
            ? projected
            : projected.Where(d => d.OrderType == orderTypeFilter.Value);

        var ordered = (orderBy ?? BO.ClosedOrdersListOrderBy.ByTypeOfCompletion) switch
        {
            BO.ClosedOrdersListOrderBy.ByTypeOfCompletion =>
                filtered
                    .OrderBy(d => d.TypeOfDeliveryCompletionTime)
                    .ThenBy(d => d.DeliveryId),

            BO.ClosedOrdersListOrderBy.ByOrderType =>
                filtered
                    .OrderBy(d => d.OrderType)
                    .ThenBy(d => d.DeliveryId),

            BO.ClosedOrdersListOrderBy.ByTiming =>
                filtered
                    .OrderBy(d => d.ActualTimeOfCompletion)
                    .ThenBy(d => d.DeliveryId),

            _ =>
                filtered
                    .OrderBy(d => d.TypeOfDeliveryCompletionTime)
                    .ThenBy(d => d.DeliveryId)
        };

        return ordered;
        
    }

    /// <summary>
    /// Retrieves a list of open orders that are within delivery range for a specific courier.
    /// </summary>
    /// <param name="requesterId">The ID of the user requesting the list (must be admin).</param>
    /// <param name="courierId">The ID of the courier to find orders for.</param>
    /// <param name="orderTypeFilter">Optional filter by order type.</param>
    /// <param name="orderBy">Optional sorting criteria for the returned list.</param>
    /// <returns>An enumerable collection of OpenOrderInList objects within the courier's range.</returns>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the requester is not an admin.</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the courier doesn't exist.</exception>
    internal static IEnumerable<BO.OpenOrderInList> GetOpenDeliveries(int requesterId,int courierId,BO.OrderType? orderTypeFilter = null,BO.OpenDeliveryListOrderBy? orderBy = null)
    {
        Tools.AuthorizeAdmin(requesterId);
        var courier = s_dal.Courier.Read(courierId);
        if (courier == null)
            throw new BO.BlDoesNotExistException($"Courier {courierId} not found");
        var orders = s_dal.Order.ReadAll();
        var openOrders = orders
            .Where(o => GetOrderStatus(o) == BO.OrderStatus.Open)
            .ToList();
        var withinRange = openOrders
            .Where(order =>
            {
                double airDistance = Tools.AirDistance(
                    order.Latitude,
                    order.Longitude,
                    AdminManager.GetConfig().Latitude.Value,
                    AdminManager.GetConfig().Longitude.Value);

                return airDistance <= courier.MaxPersonalDeliveryDistance;
            })
            .ToList();

        var projected = withinRange
            .Select(MapDoToOpenOrderInList)
            .ToList();
        var filtered = orderTypeFilter is null
            ? projected
            : projected.Where(o => o.OrderType == orderTypeFilter.Value);

        var ordered = (orderBy ?? BO.OpenDeliveryListOrderBy.ByTiming) switch
        {
            BO.OpenDeliveryListOrderBy.ByDistance =>
                filtered.OrderBy(o => o.AirDistance),

            BO.OpenDeliveryListOrderBy.ById =>
                filtered.OrderBy(o => o.OrderId),

            _ =>
                filtered.OrderBy(o => o.ScheduleStatus)
        };
        return ordered;
    }

    /// <summary>
    /// Retrieves a specific order by ID with all related information.
    /// </summary>
    /// <param name="requesterId">The ID of the user requesting the order (must be admin).</param>
    /// <param name="orderId">The ID of the order to retrieve.</param>
    /// <returns>An Order object containing the order's complete information.</returns>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the requester is not an admin.</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the order doesn't exist.</exception>
    internal static BO.Order GetOrder(int requesterId, int orderId)
    {
        Tools.AuthorizeAdmin(requesterId);

        var d = s_dal.Order.Read(orderId);
        if (d == null)
            throw new BO.BlDoesNotExistException($"Order {orderId} not found");

        return FromDoToBo(d);
    }

    /// <summary>
    /// Updates an existing order's information.
    /// </summary>
    /// <param name="requesterId">The ID of the user updating the order (must be admin).</param>
    /// <param name="order">The Order object containing updated information.</param>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the requester is not an admin.</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the order doesn't exist.</exception>
    /// <exception cref="BO.BlInvalidOperationException">Thrown if the order cannot be updated in its current status.</exception>
    internal static void UpdateOrder(int requesterId, BO.Order order)
    {
        Tools.AuthorizeAdmin(requesterId);

        var existingDoOrder = s_dal.Order.Read(order.Id);
        if (existingDoOrder == null)
            throw new BO.BlDoesNotExistException($"Order {order.Id} not found");

        var status = GetOrderStatus(existingDoOrder);
        if (status != BO.OrderStatus.Open)
            throw new BO.BlInvalidOperationException($"Cannot update order {order.Id} with status {status}");
        string newAddress = order.AddressOfOrder;
        double newLat = existingDoOrder.Latitude;
        double newLon = existingDoOrder.Longitude;

        if (!string.Equals(existingDoOrder.AddressOfOrder, newAddress, StringComparison.Ordinal))
        {
            // כתובת השתנתה, מחשבים קואורדינטות חדשות
            var coords = Tools.GetCoordinatesForAddress(newAddress);
            newLat = coords.Latitude;
            newLon = coords.Longitude;
        }

        var updatedDoOrder = new DO.Order(
            existingDoOrder.Id,
            (DO.OrderType)order.orderType,
            newAddress,
            newLat,
            newLon,
            order.CustomerName,
            order.CustomerPhone,
            order.IsFrag,
            existingDoOrder.OrderPlacementTime,  
            order.Volume,
            order.Weight,
            order.DescriptionOfOrder
        );

        s_dal.Order.Update(updatedDoOrder);

        Observers.NotifyItemUpdated(order.Id);
        Observers.NotifyListUpdated();
    }

    /// <summary>
    /// Cancels an order and creates a cancellation delivery record.
    /// </summary>
    /// <param name="requesterId">The ID of the user canceling the order (must be admin).</param>
    /// <param name="orderId">The ID of the order to cancel.</param>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the requester is not an admin.</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the order doesn't exist.</exception>
    /// <exception cref="BO.BlInvalidOperationException">Thrown if the order cannot be canceled in its current status.</exception>
    internal static void CancelOrder(int requesterId, int orderId)
    {
        Tools.AuthorizeAdmin(requesterId);
        
        var order = s_dal.Order.Read(orderId);
        if (order == null)
            throw new BO.BlDoesNotExistException($"Order {orderId} not found");

        var orderStatus = GetOrderStatus(order);
        if (orderStatus != BO.OrderStatus.Open && orderStatus != BO.OrderStatus.InProgress)
            throw new BO.BlInvalidOperationException($"Cannot cancel order with status {orderStatus}");

        if (orderStatus == BO.OrderStatus.Open)
        {
            var dummyDelivery = new DO.Delivery(
                0,
                orderId,
                0, 
                AdminManager.Now,
                DO.DeliveryType.Car,
                0, 
                AdminManager.Now, 
                DO.TypeOfDeliveryCompletionTime.Canceled
            );
            
            s_dal.Delivery.Create(dummyDelivery);
        }
        else if (orderStatus == BO.OrderStatus.InProgress)
        {
            var deliveries = s_dal.Delivery.ReadAll(d => d.OrderId == orderId && d.DeliveryCompletionTime == null);
            
            if (deliveries != null && deliveries.Any())
            {
                var activeDelivery = deliveries.First();
                
                var updatedDelivery = new DO.Delivery(
                    activeDelivery.Id,
                    activeDelivery.OrderId,
                    activeDelivery.CourierId,
                    activeDelivery.DeliveryStartTime,
                    activeDelivery.DeliveryType,
                    activeDelivery.ActualDistance,
                    AdminManager.Now,
                    DO.TypeOfDeliveryCompletionTime.Canceled
                );
                
                s_dal.Delivery.Update(updatedDelivery);
            }
        }

        Observers.NotifyItemUpdated(orderId);
        Observers.NotifyListUpdated();
    }

    /// <summary>
    /// Attempts to delete an order (operation is not allowed).
    /// </summary>
    /// <param name="requesterId">The ID of the user attempting to delete (must be admin).</param>
    /// <param name="orderId">The ID of the order to delete.</param>
    /// <exception cref="BO.BlInvalidOperationException">Always thrown as orders cannot be deleted from the system.</exception>
    internal static void DeleteOrder(int requesterId, int orderId)
    {
        Tools.AuthorizeAdmin(requesterId);
        
        throw new BO.BlInvalidOperationException("Orders cannot be deleted from the system");
    }

    /// <summary>
    /// Creates a new order in the system with geocoding for the address.
    /// </summary>
    /// <param name="requesterId">The ID of the user creating the order (must be admin).</param>
    /// <param name="order">The Order object containing the new order's information.</param>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the requester is not an admin.</exception>
    /// <exception cref="BO.BlDataValidationException">Thrown if address geocoding fails.</exception>
    internal static void CreateOrder(int requesterId, BO.Order order)
    {
        Tools.AuthorizeAdmin(requesterId);

        var (lat, lon) = Tools.GetCoordinatesForAddress(order.AddressOfOrder);
        var doOrder = new DO.Order(
            0, 
            (DO.OrderType)order.orderType,
            order.AddressOfOrder,
            lat,
            lon,
            order.CustomerName,
            order.CustomerPhone,
            order.IsFrag,
            order.OrderPlacementTime ?? AdminManager.Now,
            order.Volume,
            order.Weight,
            order.DescriptionOfOrder
        );
        
        s_dal.Order.Create(doOrder);

        Observers.NotifyListUpdated();
    }

    /// <summary>
    /// Marks a delivery as complete for a courier.
    /// </summary>
    /// <param name="requesterId">The ID of the user completing the delivery (must be the courier or admin).</param>
    /// <param name="courierId">The ID of the courier completing the delivery.</param>
    /// <param name="deliveryId">The ID of the delivery to complete.</param>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the requester is not the courier or an admin.</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the delivery doesn't exist.</exception>
    /// <exception cref="BO.BlInvalidOperationException">Thrown if the delivery doesn't belong to the courier or is already completed.</exception>
    internal static void CompleteOrderForCourier(int requesterId, int courierId, int deliveryId)
    {
        if (requesterId != courierId && !Tools.IsAdmin(requesterId))
            throw new BO.BlUnauthorizedAccessException("Not authorized to complete this delivery");
        
        var delivery = s_dal.Delivery.Read(deliveryId);
        if (delivery == null)
            throw new BO.BlDoesNotExistException($"Delivery {deliveryId} not found");

        if (delivery.CourierId != courierId)
            throw new BO.BlInvalidOperationException($"Delivery {deliveryId} does not belong to courier {courierId}");

        if (delivery.DeliveryCompletionTime.HasValue)
            throw new BO.BlInvalidOperationException($"Delivery {deliveryId} is already completed");

        var updatedDelivery = new DO.Delivery(
            delivery.Id,
            delivery.OrderId,
            delivery.CourierId,
            delivery.DeliveryStartTime,
            delivery.DeliveryType,
            delivery.ActualDistance,
            AdminManager.Now,
            DO.TypeOfDeliveryCompletionTime.Supplied
        );
        
        s_dal.Delivery.Update(updatedDelivery);

        Observers.NotifyItemUpdated(delivery.OrderId);
        Observers.NotifyListUpdated();
    }

    /// <summary>
    /// Assigns an open order to a courier for delivery.
    /// </summary>
    /// <param name="requesterId">The ID of the user assigning the order (must be admin).</param>
    /// <param name="courierId">The ID of the courier to assign the order to.</param>
    /// <param name="orderId">The ID of the order to assign.</param>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the requester is not an admin.</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the order or courier doesn't exist.</exception>
    /// <exception cref="BO.BlInvalidOperationException">Thrown if the order is not in Open status.</exception>
    internal static void ChooseOrderForDelivery(int requesterId, int courierId, int orderId)
    {
        Tools.AuthorizeAdmin(requesterId);
        
        var order = s_dal.Order.Read(orderId);
        if (order == null)
            throw new BO.BlDoesNotExistException($"Order {orderId} not found");

        var courier = s_dal.Courier.Read(courierId);
        if (courier == null)
            throw new BO.BlDoesNotExistException($"Courier {courierId} not found");

        var orderStatus = GetOrderStatus(order);
        if (orderStatus != BO.OrderStatus.Open)
            throw new BO.BlInvalidOperationException($"Order {orderId} must be Open to assign");

        var newDelivery = new DO.Delivery(
            0,
            orderId,
            courierId,
            AdminManager.Now,
            courier.DeliveryType,
            Tools.CalculateActualDistance(order.Latitude, order.Longitude, courier.DeliveryType)
        );
        
        s_dal.Delivery.Create(newDelivery);

        Observers.NotifyItemUpdated(orderId);
        Observers.NotifyListUpdated();
    }

    // ==================== Helper Methods ====================

    /// <summary>
    /// Converts a Data Access Layer Order object to a Business Logic OrderInList object.
    /// </summary>
    /// <param name="order">The DO.Order object to convert.</param>
    /// <returns>An OrderInList object with calculated metrics and current status.</returns>
    private static BO.OrderInList MapDoToOrderInList(DO.Order order)
    {
        var orderStatus = GetOrderStatus(order);
        var scheduleStatus = GetScheduleStatusOfOrder(order);
        var ExpectedDelivery = s_dal.Delivery.ReadAll(d => d.OrderId == order.Id && d.DeliveryCompletionTime == null).FirstOrDefault();

        double airDistance = Tools.AirDistance(
            order.Latitude,
            order.Longitude,
            AdminManager.GetConfig().Latitude.GetValueOrDefault(),
            AdminManager.GetConfig().Longitude.GetValueOrDefault());

        return new BO.OrderInList
        {
            DeliveryId = s_dal.Delivery.ReadAll(d => d.OrderId == order.Id).FirstOrDefault()?.Id ?? 0,
            OrderId = order.Id,
            OrderType = (BO.OrderType)order.OrderType,
            AirDistance = airDistance,
            OrderStatus = orderStatus,
            ScheduleStatus = scheduleStatus,
            RemainingTineToCompletion = orderStatus == BO.OrderStatus.Open
                ? (order.OrderPlacementTime + AdminManager.GetConfig().MaxTimeRangeForDelivery - AdminManager.Now)
                : TimeSpan.Zero,
            ExpectedTimeToCompletion = ExpectedDelivery != null && orderStatus == BO.OrderStatus.InProgress
                ? ExpectedDeliveryTime(ExpectedDelivery)
                : TimeSpan.Zero,
            AmountOfDeliveries = s_dal.Delivery.ReadAll(d => d.OrderId == order.Id).Count()
        };
    }

    /// <summary>
    /// Converts a Data Access Layer Order object to a Business Logic OpenOrderInList object.
    /// </summary>
    /// <param name="order">The DO.Order object to convert.</param>
    /// <returns>An OpenOrderInList object suitable for displaying available orders to couriers.</returns>
    private static BO.OpenOrderInList MapDoToOpenOrderInList(DO.Order order)
    {
        double airDistance = Tools.AirDistance(order.Latitude, order.Longitude,
            AdminManager.GetConfig().Latitude.GetValueOrDefault(),
            AdminManager.GetConfig().Longitude.GetValueOrDefault());

        DateTime maxDeliveryTime = order.OrderPlacementTime + AdminManager.GetConfig().MaxTimeRangeForDelivery;
        TimeSpan remainingTime = maxDeliveryTime - AdminManager.Now;

        return new BO.OpenOrderInList
        {
            OrderId = order.Id,
            OrderType = (BO.OrderType)order.OrderType,
            IsFrag = order.IsFrag,
            Weight = order.Weight,
            Volume = order.Volume,
            AddressOfOrder = order.AddressOfOrder,
            AirDistance = airDistance,
            ActualDistance = null,
            ActualTime = null,
            ScheduleStatus = GetScheduleStatusOfOrder(order),
            RemainingTimeForCompletion = remainingTime > TimeSpan.Zero ? remainingTime : TimeSpan.Zero,
            MaxDeliveryTime = maxDeliveryTime
        };
    }

    /// <summary>
    /// Converts a Data Access Layer Delivery object to a Business Logic ClosedDeliveryInList object.
    /// </summary>
    /// <param name="delivery">The DO.Delivery object to convert.</param>
    /// <returns>A ClosedDeliveryInList object containing completed delivery information.</returns>
    private static BO.ClosedDeliveryInList MapDoToClosedDeliveryInList(DO.Delivery delivery)
    {
        var order = s_dal.Order.Read(delivery.OrderId);
        if (order == null)
            throw new BO.BlDoesNotExistException($"Order {delivery.OrderId} not found");

        TimeSpan? actualTime = delivery.DeliveryCompletionTime.HasValue
            ? delivery.DeliveryCompletionTime.Value - delivery.DeliveryStartTime
            : null;

        return new BO.ClosedDeliveryInList
        {
            DeliveryId = delivery.Id,
            OrderId = delivery.OrderId,
            OrderType = (BO.OrderType)order.OrderType,
            AddressOfOrder = order.AddressOfOrder,
            DeliveryType = (BO.DeliveryType)delivery.DeliveryType,
            ActualDistance = delivery.ActualDistance,
            ActualTimeOfCompletion = actualTime,
            TypeOfDeliveryCompletionTime = (BO.TypeOfDeliveryCompletionTime?)delivery.TypeOfDeliveryCompletionTime ?? BO.TypeOfDeliveryCompletionTime.Supplied
        };
    }

    /// <summary>
    /// Determines the current operational status of an order based on its deliveries.
    /// </summary>
    /// <param name="order">The order to evaluate.</param>
    /// <returns>An OrderStatus value indicating the order's current operational status.</returns>
    internal static BO.OrderStatus GetOrderStatus(DO.Order order)
    {
        var deliveries = s_dal.Delivery.ReadAll(d => d.OrderId == order.Id);
        
        if (deliveries == null || !deliveries.Any())
            return BO.OrderStatus.Open;

        var latestDelivery = deliveries.OrderByDescending(d => d.DeliveryStartTime).FirstOrDefault();
        
        if (latestDelivery == null)
            return BO.OrderStatus.Open;

        return CourierManager.GetDeliveryStatus(latestDelivery);
    }

    /// <summary>
    /// Determines the scheduling status of an order based on whether it's on-time or late.
    /// </summary>
    /// <param name="order">The order to evaluate.</param>
    /// <returns>A ScheduleStatus value indicating the order's timing status.</returns>
    internal static BO.ScheduleStatus GetScheduleStatusOfOrder(DO.Order order)
    {
        var deliveries = s_dal.Delivery.ReadAll(d => d.OrderId == order.Id);
        
        if (deliveries == null || !deliveries.Any())
        {
            DateTime maxDeliveryTime = order.OrderPlacementTime + AdminManager.GetConfig().MaxTimeRangeForDelivery;
            return AdminManager.Now > maxDeliveryTime ? BO.ScheduleStatus.Late : BO.ScheduleStatus.OnTime;
        }

        var latestDelivery = deliveries.OrderByDescending(d => d.DeliveryStartTime).FirstOrDefault();
        
        if (latestDelivery == null)
        {
            DateTime maxDeliveryTime = order.OrderPlacementTime + AdminManager.GetConfig().MaxTimeRangeForDelivery;
            return AdminManager.Now > maxDeliveryTime ? BO.ScheduleStatus.Late : BO.ScheduleStatus.OnTime;
        }

        return CourierManager.GetScheduleStatusOfDelivery(latestDelivery);
    }

    /// <summary>
    /// Converts a Data Access Layer Order object to a Business Logic Order object.
    /// </summary>
    /// <param name="d">The DO.Order object to convert.</param>
    /// <returns>A BO.Order object with calculated metrics and delivery history.</returns>
    private static BO.Order FromDoToBo(DO.Order d)
    {
        // Get all deliveries for this order
        var deliveries = s_dal.Delivery.ReadAll(del => del.OrderId == d.Id);
        
        // Convert deliveries to DeliveryPerOrderInList objects
        IEnumerable<BO.DeliveryPerOrderInList>? deliveryList = null;
        if (deliveries != null && deliveries.Any())
        {
            deliveryList = deliveries.Select(del => MapDeliveryToPerOrderInList(del)).ToList();
        }

        // Get the latest active delivery for calculations
        var activeDelivery = deliveries?
            .Where(del => del.DeliveryCompletionTime == null)
            .OrderByDescending(del => del.DeliveryStartTime)
            .FirstOrDefault();

        DateTime? expectedCompletion = null;
        if (activeDelivery != null)
        {
            expectedCompletion = activeDelivery.DeliveryStartTime + ExpectedDeliveryTime(activeDelivery);
        }

        DateTime maxDeliveryTime = d.OrderPlacementTime + AdminManager.GetConfig().MaxTimeRangeForDelivery;
        
        TimeSpan remainingTime = TimeSpan.Zero;
        if (activeDelivery != null && expectedCompletion.HasValue)
        {
            remainingTime = expectedCompletion.Value - AdminManager.Now;
            if (remainingTime < TimeSpan.Zero)
                remainingTime = TimeSpan.Zero;
        }

        return new BO.Order
        {
            Id = d.Id,
            orderType = (BO.OrderType)d.OrderType,
            DescriptionOfOrder = d.DescriptionOfOrder,
            AddressOfOrder = d.AddressOfOrder,
            Latitude = d.Latitude,
            Longitude = d.Longitude,
            CustomerName = d.CustomerName,
            CustomerPhone = d.CustomerPhone,
            IsFrag = d.IsFrag,
            Volume = d.Volume,
            Weight = d.Weight,
            OrderPlacementTime = d.OrderPlacementTime,
            ExpectedCompletionTime = expectedCompletion,
            MaxDeliveryTime = maxDeliveryTime,
            OrderStatus = GetOrderStatus(d),
            ScheduleStatus = GetScheduleStatusOfOrder(d),
            RemainingTimeToDelivery = AdminManager.Now.Add(remainingTime),
            AirDistance = Tools.AirDistance(d.Latitude, d.Longitude,
                AdminManager.GetConfig().Latitude.GetValueOrDefault(),
                AdminManager.GetConfig().Longitude.GetValueOrDefault()),
            DeliveriesForOrder = deliveryList
        };
    }

    /// <summary>
    /// Converts a DO.Delivery to a BO.DeliveryPerOrderInList object.
    /// </summary>
    /// <param name="delivery">The delivery to convert.</param>
    /// <returns>A DeliveryPerOrderInList object with courier information.</returns>
    private static BO.DeliveryPerOrderInList MapDeliveryToPerOrderInList(DO.Delivery delivery)
    {
        var courier = s_dal.Courier.Read(delivery.CourierId);
        
        return new BO.DeliveryPerOrderInList
        {
            DeliveryId = delivery.Id,
            CourierId = delivery.CourierId == 0 ? null : delivery.CourierId,
            CourierName = courier?.FullName ?? "לא משוייך",
            DeliveryType = (BO.DeliveryType)delivery.DeliveryType,
            DeliveryStartTime = delivery.DeliveryStartTime,
            TypeOfDeliveryCompletionTime = (BO.TypeOfDeliveryCompletionTime?)delivery.TypeOfDeliveryCompletionTime ?? BO.TypeOfDeliveryCompletionTime.Supplied,
            DeliveryEndTime = delivery.DeliveryCompletionTime ?? default
        };
    }

    /// <summary>
    /// Calculates the expected delivery time based on the delivery distance and courier's average speed.
    /// </summary>
    /// <param name="delivery">The delivery to calculate time for.</param>
    /// <returns>A TimeSpan representing the expected duration of the delivery.</returns>
    internal static TimeSpan ExpectedDeliveryTime(DO.Delivery delivery)
    {
        double averageSpeed = CourierManager.GetAverageSpeedKmh(delivery.DeliveryType);
        double distance = delivery.ActualDistance ?? 0;
        double hours = averageSpeed > 0 ? distance / averageSpeed : 0;
        return TimeSpan.FromHours(hours);
    }

    /// <summary>
    /// Converts a Data Access Layer Delivery object to a Business Logic OrderInProgress object.
    /// </summary>
    /// <param name="order">The DO.Order object for the delivery.</param>
    /// <param name="activeDelivery">The DO.Delivery object in progress.</param>
    /// <returns>An OrderInProgress object containing complete delivery tracking information.</returns>
    internal static BO.OrderInProgress AddOrderInProgress(DO.Order order, DO.Delivery activeDelivery)
    {
        return new BO.OrderInProgress
        {
            DeliveryId = activeDelivery.Id,
            OrderId = order.Id,
            OrderType = (BO.OrderType)order.OrderType,
            DescriptionOfOrder = order.DescriptionOfOrder,
            AddressOfOrder = order.AddressOfOrder,
            AirDistance = Tools.AirDistance(order.Latitude, order.Longitude,
                AdminManager.GetConfig().Latitude.GetValueOrDefault(),
                    AdminManager.GetConfig().Longitude.GetValueOrDefault()),
            ActualDistance = activeDelivery.ActualDistance,
            CustomerName = order.CustomerName,
            CustomerPhone = order.CustomerPhone,
            OrderPlacementTime = order.OrderPlacementTime,
            PickUpTime = activeDelivery.DeliveryStartTime,
            DeliveryTime = activeDelivery.DeliveryStartTime + ExpectedDeliveryTime(activeDelivery),
            MaxDelivryTime = order.OrderPlacementTime + AdminManager.GetConfig().MaxTimeRangeForDelivery,
            OrderStatus = CourierManager.GetDeliveryStatus(activeDelivery),
            ScheduleStatus = CourierManager.GetScheduleStatusOfDelivery(activeDelivery),
            TimeLeftToDelivery = (activeDelivery.DeliveryStartTime + AdminManager.GetConfig().MaxTimeRangeForDelivery) - AdminManager.Now
        };
    }
}
