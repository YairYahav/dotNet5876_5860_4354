
namespace Dal;
using DalApi;
using DO;

/// <summary>
/// Implementation of the IOrder interface for managing Order entities in the Data Access Layer (DAL).
/// </summary>

internal class OrderImplementation : IOrder
{


    /// <summary>
    /// Creates new Order entity object in DAL with a new unique Id
    /// </summary>
    /// <param name="item"></param>
    public void Create(Order item)
    {
        int newID = Config.NextOrderId;
        Order newOrder = item with { Id = newID };
        DataSource.Orders.Add(newOrder);
    }


    /// <summary>
    /// Deletes an object by its Id if it exists but if it does not, throws an InvalidOperationException.
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Delete(int id)
    {
        if (Read(id) == null)
            throw new InvalidOperationException($"Order with Id {id} does not exist");
        DataSource.Orders.RemoveAll(o => o?.Id == id);
    }


    /// <summary>
    /// Delete all entity objects.
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Orders.Clear();
    }

    /// <summary>
    /// Reads entity object by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns> The Order object with the specified ID, or null if not found. </returns>
    public Order? Read(int id)
    {
        return DataSource.Orders.FirstOrDefault(c => c?.Id == id);
    }

    /// <summary>
    /// Reads all entity objects.
    /// </summary>
    /// <returns> The list of all Order objects. </returns>
    public IEnumerable<Order> ReadAll()
    {
        return DataSource.Orders.Where(o => o is not null)!;
    }


    /// <summary>
    /// Updates entity object if it exists but if it does not, throws an InvalidOperationException.
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Update(Order item)
    {
        if (Read(item.Id) == null)
            throw new InvalidOperationException($"Order with Id {item.Id} does not exist");
        DataSource.Orders.RemoveAll(o => o?.Id == item.Id);
        DataSource.Orders.Add(item);
    }
}
