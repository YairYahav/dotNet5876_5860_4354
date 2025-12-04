using BlApi;
using BO;
using Helpers;

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

    // Stage 5:
    public void AddObserver(Action listObserver) => CourierManager.Observers.AddListObserver(listObserver);
    public void RemoveObserver(Action listObserver) => CourierManager.Observers.RemoveListObserver(listObserver);
    public void AddObserver(int id, Action observer) => CourierManager.Observers.AddObserver(id, observer);
    public void RemoveObserver(int id, Action observer) => CourierManager.Observers.RemoveObserver(id, observer);
}
