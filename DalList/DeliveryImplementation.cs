
namespace Dal;
using DalApi;
using DO;
using System.Linq;
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
        if (Read(item.Id) != null)
            throw new DalAlreadyExistsException($"Delivery with Id {item.Id} already exists");

        DataSource.Deliveries.Add(item);
    }


    /// <summary>
    /// Deletes an object by its Id if it exists but if it does not, throws an InvalidOperationException.
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Delete(int id)
    {
        if (Read(id) == null)
            throw new DalDoesNotExistException($"Delivery with Id {id} does not exist");

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
        var delivery = DataSource.Deliveries.FirstOrDefault(d => d?.Id == id);
        if (delivery == null)
            throw new DalDoesNotExistException($"Delivery with Id {id} does not exist");

        return delivery;
    }


    public Delivery? Read(Func<Delivery, bool> filter)
    {
        if (filter == null)
            throw new DalNullReferenceException("Filter function cannot be null");

        var delivery = DataSource.Deliveries.FirstOrDefault(filter);
        if (delivery == null)
            throw new DalDoesNotExistException("No Delivery matches the filter");

        return delivery;
    }

    /// <summary>
    /// Reads all entity objects.
    /// </summary>
    /// <returns> The list of all Delivery objects. </returns>
    public IEnumerable<Delivery> ReadAll(Func<Delivery, bool>? filter = null) // Stage 2
    {
        return filter == null
            ? DataSource.Deliveries.Select(item => item)
            : DataSource.Deliveries.Where(filter);
    }

    /// <summary>
    /// Updates entity object if it exists but if it does not, throws an InvalidOperationException.
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Update(Delivery item)
    {
        var existing = DataSource.Deliveries.FirstOrDefault(d => d?.Id == item.Id);
        if (existing == null)
            throw new DalDoesNotExistException($"Delivery with Id {item.Id} does not exist");

        DataSource.Deliveries.RemoveAll(d => d?.Id == item.Id);
        DataSource.Deliveries.Add(item);
    }
}
