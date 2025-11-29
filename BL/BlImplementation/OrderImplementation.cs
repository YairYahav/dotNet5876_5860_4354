using BlApi;
using BO;


namespace BlImplementation;

internal class OrderImplementation : IOrder
{
    public void CancelOrder(int requesterId, int orderId)
    {
        Helpers.OrderManager.CanceleOrder(requesterId, orderId);
        
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

    public IEnumerable<ClosedDeliveryInList> GerClosedDeliveries(int requesterId, int courierId, ClosedDeliveryListFilterBy? filterBy = null, ClosedDeliveryListOrderBy? orderBy = null)
    {
        Helpers.OrderManager.GerClosedDeliveries(requesterId, courierId);
    }

    public IEnumerable<OpenOrderInList> GetOpenDeliveries(int requesterId, int courierId, OpenDeliveryListFilterBy? filterBy = null, OpenDeliveryListOrderBy? orderBy = null)
    {
        Helpers.OrderManager.GetOpenDeliveries(requesterId, courierId);
    }

    public Order GetOrder(int requesterId, int orderId)
    {
        Helpers.OrderManager.GetOrder(requesterId, orderId);
    }

    public IEnumerable<OpenOrderInList> GetOrders(int requesterId, OrderListFilterBy? filterBy = null, object? filterValue = null, OrderListOrderBy? orderBy = null)
    {
        Helpers.OrderManager.GetOrders(requesterId, filterBy, filterValue, orderBy);
    }

    public int[] GetOrdersSummary(int requesterId)
    {
        Helpers.OrderManager.GetOrdersSummary(requesterId);
    }

    public void UpdateOrder(int requesterId, Order order)
    {
        Helpers.OrderManager.UpdateOrder(requesterId, order);
    }
}
