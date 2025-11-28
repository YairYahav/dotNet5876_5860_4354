using DalApi;
//using DO = DO;
//using BO = BO;

namespace Helpers;

internal static class OrderManager
{
    private static readonly IDal s_dal = Factory.Get;
    internal static int[] GetOrdersSummery(int requesterId)
    {
        AuthorizeAdmin(requesterId);
        var orders = s_dal.Order.ReadAll().ToList();
        var projected = orders.Select(o => (Status: GetOrderStatus(o), Timing: GetTimingStatus(o)));
        var grouped = projected
            .GroupBy(x => x)
            .ToDictionary(g => g.Key, g => g.Count());
        var statuses = Enum.GetValues(typeof(BO.OrderStatus))
                     .Cast<BO.OrderStatus>()
                     .ToArray();
        var timings = Enum.GetValues(typeof(BO.ScheduleStatus))
                           .Cast<BO.ScheduleStatus>()
                           .ToArray();


    }

    private static void AuthorizeAdmin(int requesterId)
    {
        var u = s_dal.Config.ManagerId;
        if (requesterId != s_dal.Config.ManagerId)
            throw new BO.BlUnauthorizedAccessException("Only admin users are authorized to perform this action.");
    }

    private static BO.OrderStatus GetOrderStatus(DO.Order o)
    {
    }
    private static BO.ScheduleStatus GetTimingStatus(DO.Order o)
    {
    }

}