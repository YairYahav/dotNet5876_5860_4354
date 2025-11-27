using BlApi;
using BO;

//using BO; // מעדיף לכתוב לפני טיפוסים הלוגים BO כדי לא להתבלבל עם DO

namespace BlImplementation;

internal class CourierImplementation : ICourier
{
    public void CreateCourier(int requesterId, Courier courier)
    {
        throw new NotImplementedException();
    }

    public void DeleteCourier(int requesterId, int courierId)
    {
        throw new NotImplementedException();
    }

    public Courier GetCourier(int requesterId, int courierId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<CourierInList> GetCouriers(int requesterId, bool? onlyActive = null, CourierListOrderBy? orderBy = null)
    {
        throw new NotImplementedException();
    }

    public void Login(string id, string password)
    {
        throw new NotImplementedException();
    }

    public void UpdateCourier(int requesterId, Courier courier)
    {
        throw new NotImplementedException();
    }
}
