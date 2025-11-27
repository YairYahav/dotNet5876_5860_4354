using DalApi;
using DO = DO;
//using BO = BO;

namespace Helpers;

internal static class DeliveryManager
{
    private static readonly IDal s_dal = Factory.Get;
    internal static IEnumerable<BO.CourierInList> GetCouriers

}