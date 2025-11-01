public interface IDelivery
{
    void Create(Delivery item);
    Delivery? Read(int id);
    List<Delivery> ReadAll();
    void Update(Delivery item);
    void Delete(int id);
    void DeleteAll();
}