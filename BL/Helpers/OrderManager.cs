using DalApi;
using DO;
using BO;

namespace Helpers;

internal static class OrderManager
{
    private static readonly IDal s_dal = Factory.Get;

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
                    s_dal.Config.Latitude.GetValueOrDefault(),
                    s_dal.Config.Longitude.GetValueOrDefault());

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

    internal static BO.Order GetOrder(int requesterId, int orderId)
    {
        Tools.AuthorizeAdmin(requesterId);

        var d = s_dal.Order.Read(orderId);
        if (d == null)
            throw new BO.BlDoesNotExistException($"Order {orderId} not found");

        return FromDoToBo(d);
    }

    internal static void UpdateOrder(int requesterId, BO.Order order)
    {
        Tools.AuthorizeAdmin(requesterId);

        var existingDoOrder = s_dal.Order.Read(order.Id);
        if (existingDoOrder == null)
            throw new BO.BlDoesNotExistException($"Order {order.Id} not found");

        var status = GetOrderStatus(existingDoOrder);
        if (status != BO.OrderStatus.Open)
            throw new BO.BlInvalidOperationException(
                $"Cannot update order {order.Id} with status {status}"
            );
        var updatedDoOrder = new DO.Order(
            existingDoOrder.Id,
            (DO.OrderType)order.orderType,
            order.AddressOfOrder,
            order.Latitude,
            order.Longitude,
            order.CustomerName,
            order.CustomerPhone,
            order.IsFrag,
            existingDoOrder.OrderPlacementTime,  
            order.Volume,
            order.Weight,
            order.DescriptionOfOrder
        );

        s_dal.Order.Update(updatedDoOrder);
    }

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
    }

    internal static void DeleteOrder(int requesterId, int orderId)
    {
        Tools.AuthorizeAdmin(requesterId);
        
        throw new BO.BlInvalidOperationException("Orders cannot be deleted from the system");
    }

    internal static void CreateOrder(int requesterId, BO.Order order)
    {
        Tools.AuthorizeAdmin(requesterId);
        var doOrder = new DO.Order(
            0, 
            (DO.OrderType)order.orderType,
            order.AddressOfOrder,
            order.Latitude,
            order.Longitude,
            order.CustomerName,
            order.CustomerPhone,
            order.IsFrag,
            order.OrderPlacementTime ?? AdminManager.Now,
            order.Volume,
            order.Weight,
            order.DescriptionOfOrder
        );
        
        s_dal.Order.Create(doOrder);
    }

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
    }

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
    }

    // ==================== Helper Methods ====================
    private static BO.OrderInList MapDoToOrderInList(DO.Order order)
    {
        var orderStatus = GetOrderStatus(order);
        var scheduleStatus = GetScheduleStatusOfOrder(order);
        var ExpectedDelivery = s_dal.Delivery.ReadAll(d => d.OrderId == order.Id && d.DeliveryCompletionTime == null).FirstOrDefault();

        double airDistance = Tools.AirDistance(
            order.Latitude,
            order.Longitude,
            s_dal.Config.Latitude.GetValueOrDefault(),
            s_dal.Config.Longitude.GetValueOrDefault());

        return new BO.OrderInList
        {
            DeliveryId = s_dal.Delivery.ReadAll(d => d.OrderId == order.Id).FirstOrDefault()?.Id ?? 0,
            OrderId = order.Id,
            OrderType = (BO.OrderType)order.OrderType,
            AirDistance = airDistance,
            OrderStatus = orderStatus,
            ScheduleStatus = scheduleStatus,
            RemainingTineToCompletion = orderStatus == BO.OrderStatus.Open
                ? (order.OrderPlacementTime + s_dal.Config.MaxTimeRangeForDelivery - AdminManager.Now)
                : TimeSpan.Zero,
            ExpectedTimeToCompletion = ExpectedDelivery != null && orderStatus == BO.OrderStatus.InProgress
                ? ExpectedDeliveryTime(ExpectedDelivery)
                : TimeSpan.Zero,
            AmountOfDeliveries = s_dal.Delivery.ReadAll(d => d.OrderId == order.Id).Count()
        };
    }

    private static BO.OpenOrderInList MapDoToOpenOrderInList(DO.Order order)
    {
        double airDistance = Tools.AirDistance(order.Latitude, order.Longitude,
            s_dal.Config.Latitude.GetValueOrDefault(),
            s_dal.Config.Longitude.GetValueOrDefault());

        DateTime maxDeliveryTime = order.OrderPlacementTime + s_dal.Config.MaxTimeRangeForDelivery;
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

    internal static BO.ScheduleStatus GetScheduleStatusOfOrder(DO.Order order)
    {
        var deliveries = s_dal.Delivery.ReadAll(d => d.OrderId == order.Id);
        
        if (deliveries == null || !deliveries.Any())
        {
            DateTime maxDeliveryTime = order.OrderPlacementTime + s_dal.Config.MaxTimeRangeForDelivery;
            return AdminManager.Now > maxDeliveryTime ? BO.ScheduleStatus.Late : BO.ScheduleStatus.OnTime;
        }

        var latestDelivery = deliveries.OrderByDescending(d => d.DeliveryStartTime).FirstOrDefault();
        
        if (latestDelivery == null)
        {
            DateTime maxDeliveryTime = order.OrderPlacementTime + s_dal.Config.MaxTimeRangeForDelivery;
            return AdminManager.Now > maxDeliveryTime ? BO.ScheduleStatus.Late : BO.ScheduleStatus.OnTime;
        }

        return CourierManager.GetScheduleStatusOfDelivery(latestDelivery);
    }

    private static BO.Order FromDoToBo(DO.Order d)
    {
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
            AirDistance = Tools.AirDistance(d.Latitude, d.Longitude,
                s_dal.Config.Latitude.GetValueOrDefault(),
                s_dal.Config.Longitude.GetValueOrDefault())
        };
    }

    internal static TimeSpan ExpectedDeliveryTime(DO.Delivery delivery)
    {
        double averageSpeed = CourierManager.GetAverageSpeedKmh(delivery.DeliveryType);
        double distance = delivery.ActualDistance ?? 0;
        double hours = averageSpeed > 0 ? distance / averageSpeed : 0;
        return TimeSpan.FromHours(hours);
    }

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
                s_dal.Config.Latitude.GetValueOrDefault(),
                s_dal.Config.Longitude.GetValueOrDefault()),
            ActualDistance = activeDelivery.ActualDistance,
            CustomerName = order.CustomerName,
            CustomerPhone = order.CustomerPhone,
            OrderPlacementTime = order.OrderPlacementTime,
            PickUpTime = activeDelivery.DeliveryStartTime,
            DeliveryTime = activeDelivery.DeliveryStartTime + ExpectedDeliveryTime(activeDelivery),
            MaxDelivryTime = order.OrderPlacementTime + s_dal.Config.MaxTimeRangeForDelivery,
            OrderStatus = CourierManager.GetDeliveryStatus(activeDelivery),
            ScheduleStatus = CourierManager.GetScheduleStatusOfDelivery(activeDelivery),
            TimeLeftToDelivery = (activeDelivery.DeliveryStartTime + s_dal.Config.MaxTimeRangeForDelivery) - AdminManager.Now
        };
    }
}
