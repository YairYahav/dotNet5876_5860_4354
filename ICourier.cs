public interface ICourier
{
    public void Create(Courier item);
    public Courier? Read(int id);
    public List<Courier> ReadAll();
    public void Update(Courier item);
    public public void Delete(int id);
    public void DeleteAll();
}