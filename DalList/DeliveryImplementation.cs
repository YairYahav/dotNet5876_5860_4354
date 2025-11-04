
namespace Dal;
using DalApi;
using DO;
/// <summary>
/// Implementation of the IDelivery interface for managing Delivery entities in the Data Access Layer (DAL).`
/// </summary>

internal class DeliveryImplementation : IDelivery
{

    /// <summary>
    /// Creates new Delivery entity object in DAL with a new unique Id 
    /// </summary>
    /// <param name="item"></param>
    public void Create(Delivery item)
    {
        int newID = Config.NextDeliveryId;
        Delivery newDelivery = item with { Id =  newID };
        DataSource.Deliveries.Add(newDelivery);
    }


    /// <summary>
    /// Deletes an object by its Id if it exists but if it does not, throws an InvalidOperationException.
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Delete(int id)
    {
        if (Read(id) == null)
            throw new InvalidOperationException($"Delivery with Id {id} does not exist");
        DataSource.Deliveries.RemoveAll(d => d?.Id == id);
    }


    /// <summary>
    /// Delete all entity objects.
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Deliveries.Clear();
    }


    /// <summary>
    /// Reads entity object by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns> The Delivery object with the specified ID, or null if not found. </returns>
    public Delivery? Read(int id)
    {
        return DataSource.Deliveries.FirstOrDefault(c => c?.Id == id);
    }

    /// <summary>
    /// Reads all entity objects.
    /// </summary>
    /// <returns> The list of all Delivery objects. </returns>
    public IEnumerable<Delivery> ReadAll()
    {
        return DataSource.Deliveries.Where(d => d is not null)!;
    }

    /// <summary>
    /// Updates entity object if it exists but if it does not, throws an InvalidOperationException.
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Update(Delivery item)
    {
        if(Read(item.Id) == null)
            throw new InvalidOperationException($"Delivery with Id {item.Id} does not exist");
        DataSource.Deliveries.RemoveAll(d => d?.Id == item.Id);
        DataSource.Deliveries.Add(item);
    }
}
