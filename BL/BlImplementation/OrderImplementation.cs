using BlApi;
using BO;

//using BO; // מעדיף לכתוב לפני טיפוסים הלוגים BO כדי לא להתבלבל עם DO

namespace BlImplementation;

internal class OrderImplementation : IOrder
{
    public void CancelOrder(int requesterId, int orderId)
    {
        throw new NotImplementedException();
    }

    public void ChooseOrderForDelivery(int requesterId, int courierId, int orderId)
    {
        throw new NotImplementedException();
    }

    public void CompleteOrderForCourier(int requesterId, int courierId, int deliveryId)
    {
        throw new NotImplementedException();
    }

    public void CreateOrder(int requesterId, Order order)
    {
        throw new NotImplementedException();
    }

    public void DeleteOrder(int requesterId, int orderId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ClosedDeliveryInList> GerClosedDeliveries(int requesterId, int courierId, ClosedDeliveryListFilterBy? filterBy = null, ClosedDeliveryListOrderBy? orderBy = null)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<OpenOrderInList> GetOpenDeliveries(int requesterId, int courierId, OpenDeliveryListFilterBy? filterBy = null, OpenDeliveryListOrderBy? orderBy = null)
    {
        throw new NotImplementedException();
    }

    public Order GetOrder(int requesterId, int orderId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<OpenOrderInList> GetOrders(int requesterId, OrderListFilterBy? filterBy = null, object? filterValue = null, OrderListOrderBy? orderBy = null)
    {
        throw new NotImplementedException();
    }

    public int[] GetOrdersSummary(int requesterId)
    {
        throw new NotImplementedException();
    }

    public void UpdateOrder(int requesterId, Order order)
    {
        throw new NotImplementedException();
    }
}
