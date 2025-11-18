using DalApi;
using DO = DO;
using BO = BO;

namespace Helpers;

internal static class OrderManager
{
    private static readonly IDal s_dal = Factory.Get;
}