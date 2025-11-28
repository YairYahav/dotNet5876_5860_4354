using BlApi;
using BO;
using Helpers;

//using BO; // מעדיף לכתוב לפני טיפוסים הלוגים BO כדי לא להתבלבל עם DO

namespace BlImplementation;

internal class CourierImplementation : ICourier
{
    public void CreateCourier(int requesterId, Courier courier)
    {
        CourierManager.CreateCourier(requesterId, courier);
    }

    public void DeleteCourier(int requesterId, int courierId)
    {
        CourierManager.DeleteCourier(requesterId, courierId);   
    }

    public Courier GetCourier(int requesterId, int courierId)
    {
        return CourierManager.GetCourier(requesterId, courierId);
    }

    public IEnumerable<CourierInList> GetCouriers(int requesterId, bool? onlyActive = null, CourierListOrderBy? orderBy = null)
    {
        return CourierManager.GetCouriers(requesterId, onlyActive, orderBy);
    }

    public UserRole Login(int id, string password)
    {
        return CourierManager.Login(id, password);
    }

    public void UpdateCourier(int requesterId, Courier courier)
    {
        CourierManager.UpdateCourier(requesterId, courier);
    }
}
