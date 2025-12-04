using BlApi;
using BO;
using Helpers;


namespace BlImplementation;

internal class OrderImplementation : IOrder
{
    public void CancelOrder(int requesterId, int orderId)
    {
        Helpers.OrderManager.CancelOrder(requesterId, orderId);

    }

    public void ChooseOrderForDelivery(int requesterId, int courierId, int orderId)
    {
        Helpers.OrderManager.ChooseOrderForDelivery(requesterId, courierId, orderId);
    }

    public void CompleteOrderForCourier(int requesterId, int courierId, int deliveryId)
    {
        Helpers.OrderManager.CompleteOrderForCourier(requesterId, courierId, deliveryId);
    }

    public void CreateOrder(int requesterId, Order order)
    {
        Helpers.OrderManager.CreateOrder(requesterId, order);
    }

    public void DeleteOrder(int requesterId, int orderId)
    {
        Helpers.OrderManager.DeleteOrder(requesterId, orderId);
    }

    public IEnumerable<ClosedDeliveryInList> GerClosedOrders(int requesterId, int courierId, OrderType? filterBy = null, ClosedOrdersListOrderBy? orderBy = null)
    {
        return Helpers.OrderManager.GetClosedDeliveries(requesterId, courierId, filterBy, orderBy);
    }

    public IEnumerable<OpenOrderInList> GetOpenOrders(int requesterId, int courierId, OrderType? filterBy = null, OpenDeliveryListOrderBy? orderBy = null)
    {
        return Helpers.OrderManager.GetOpenDeliveries(requesterId, courierId, filterBy, orderBy);
    }

    public Order GetOrder(int requesterId, int orderId)
    {
        return Helpers.OrderManager.GetOrder(requesterId, orderId);
    }

    public IEnumerable<OrderInList> GetOrders(int requesterId, OrderListFilterBy? filterBy = null, object? filterValue = null, OrderListOrderBy? orderBy = null)
    {
        return Helpers.OrderManager.GetOrders(requesterId, filterBy, filterValue, orderBy);
    }

    public int[] GetOrdersSummary(int requesterId)
    {
        return Helpers.OrderManager.GetOrdersSummary(requesterId);
    }

    public void UpdateOrder(int requesterId, Order order)
    {
        Helpers.OrderManager.UpdateOrder(requesterId, order);
    }


    //Stage 5:
    public void AddObserver(Action listObserver) => OrderManager.Observers.AddListObserver(listObserver);
    public void RemoveObserver(Action listObserver) => OrderManager.Observers.RemoveListObserver(listObserver);
    public void AddObserver(int id, Action observer) => OrderManager.Observers.AddObserver(id, observer);
    public void RemoveObserver(int id, Action observer) => OrderManager.Observers.RemoveObserver(id, observer);
}