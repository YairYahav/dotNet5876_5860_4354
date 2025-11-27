using BO;
using DalApi;
//using DO = DO;
//using BO = BO;

namespace Helpers;


internal static class CourierManager
{
    private static readonly IDal s_dal = Factory.Get;

    internal static BO.UserRole Login(int id, string password)
    {
        if (password is null)
            throw new BO.BlDoesNotExistException("Password cannot be null or empty");

        var courier = s_dal.Courier.Read(id);
        if (courier == null)
            throw new BO.BlDoesNotExistException($"Courier with ID {id} does not exist.");

        if (s_dal.Config.ManagerId == id && s_dal.Config.ManagerPassword == password)//צריך להוסיף בדיקת הצפנה
        {
            return UserRole.Admin;
        }
        if(courier.Password == password)
        {
            return UserRole.Courier;
        }
        throw new BO.BlDoesNotExistException("Incorrect password.");
    }


    internal static IEnumerable<BO.CourierInList> GetCouriers(int requesterId, bool? onlyActive = null, BO.CourierListOrderBy? orderBy = null)
    {

    }

}
