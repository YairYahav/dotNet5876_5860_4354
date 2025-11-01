
namespace Dal;
using DalApi;
using DO;

/// <summary>
/// Implementation of the ICourier interface for managing Courier entities in the Data Access Layer (DAL).  
/// </summary>
public class CourierImplementation : ICourier
{
    /// <summary>
    /// Creates new entity object in DAL if it does not already exist but if it does, throws an InvalidOperationException.
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Create(Courier item) 
    {
        if (Read(item.Id) != null)
            throw new InvalidOperationException($"Courier with Id {item.Id} already exists");
        DataSource.Couriers.Add(item);
    }

    /// <summary>
    /// Deletes an object by its Id if it exists but if it does not, throws an InvalidOperationException.
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Delete(int id)
    {
        if(Read(id) == null)
            throw new InvalidOperationException($"Courier with Id {id} does not exist");
        DataSource.Couriers.RemoveAll(c => c?.Id == id);
    }

    /// <summary>
    /// Delete all entity objects.
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Couriers.Clear();
    }

    /// <summary>
    /// Reads entity object by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns> The Courier object with the specified ID, or null if not found. </returns>
    public Courier? Read(int id)
    {
        return DataSource.Couriers.FirstOrDefault(c => c?.Id == id);
    }

    /// <summary>
    /// Reads all entity objects.
    /// </summary>
    /// <returns> The list of all Courier objects. </returns>
    public List<Courier> ReadAll()
    {
        return new List<Courier>(DataSource.Couriers.Where(c => c is not null)!);
    }

    /// <summary>
    /// Updates entity object if it exists but if it does not, throws an InvalidOperationException.
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Update(Courier item)
    {
        if(Read(item.Id) == null)
            throw new InvalidOperationException($"Courier with Id {item.Id} does not exist");
        DataSource.Couriers.RemoveAll(c => c?.Id == item.Id);
        DataSource.Couriers.Add(item);
    }
}
