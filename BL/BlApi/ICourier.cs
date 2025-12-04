//using BO;

namespace BlApi;

public interface ICourier : IObservable
{
    BO.UserRole Login(int id, string password);

    IEnumerable<BO.CourierInList> GetCouriers(
        int requesterId, bool? onlyActive = null,
        BO.CourierListOrderBy? orderBy = null);

    BO.Courier GetCourier(int requesterId, int courierId);
    void UpdateCourier(int requesterId, BO.Courier courier);
    void CreateCourier(int requesterId, BO.Courier courier);
    void DeleteCourier(int requesterId, int courierId);
}