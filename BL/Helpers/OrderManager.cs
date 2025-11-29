using DalApi;
//using DO = DO;
//using BO = BO;

namespace Helpers;

internal static class OrderManager
{
    private static readonly IDal s_dal = Factory.Get;


    private static void AuthorizeAdmin(int requesterId)
    {
        var u = s_dal.Config.ManagerId;
        if (requesterId != s_dal.Config.ManagerId)
            throw new BO.BlUnauthorizedAccessException("Only admin users are authorized to perform this action.");
    }

    internal static BO.Order GetOrder(int requesterId, int orderId)
    {
        AuthorizeAdmin(requesterId);

        var d = s_dal.Order.Read(orderId);
            if(d == null)
                throw new BO.BlDoesNotExistException($"Order {orderId} not found");

        return FromDoToBo(d);
    }

    internal static void UpdateOrder(int requesterId, BO.Order order)
    {
        AuthorizeAdmin(requesterId);
        var existingDoOrder = s_dal.Order.Read(order.Id);
            if(existingDoOrder == null)
                throw new BO.BlDoesNotExistException($"Order {order.Id} not found");



    }
    // Help functions



    public static BO.Order FromDoToBo(DO.Order d)
    {
    }
    private static BO.OrderStatus GetOrderStatus(DO.Order o)
    {
    }
    private static BO.ScheduleStatus GetScheduleStatus(DO.Order o)
    {
    }

}