
namespace Dal;
using DalApi;
using DO;
using System.Linq;

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
        if (Read(item.Id) == null)
            throw new DalAlreadyExistsException($"Order with id {item.Id} already exist");

        DataSource.Orders.Add(item);
    }


    /// <summary>
    /// Deletes an object by its Id if it exists but if it does not, throws an InvalidOperationException.
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Delete(int id)
    {
        if (Read(id) == null)
            throw new DalDoesNotExistException($"Order with Id {id} does not exist");

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
        var order = DataSource.Orders.FirstOrDefault(o => o?.Id == id);
        if (order == null)
            throw new DalDoesNotExistException($"Order with Id {id} does not exist");

        return order;
    }


    public Order? Read(Func<Order, bool> filter)
    {
        if (filter == null)
            throw new DalNullReferenceException("Filter function cannot be null");

        var order = DataSource.Orders.FirstOrDefault(filter);
        if (order == null)
            throw new DalDoesNotExistException("No Order matches the filter");

        return order;
    }

    /// <summary>
    /// Reads all entity objects.
    /// </summary>
    /// <returns> The list of all Order objects. </returns>
    public IEnumerable<Order> ReadAll(Func<Order, bool>? filter = null) // Stage 2
    {
        return filter == null
            ? DataSource.Orders.Select(item => item)
            : DataSource.Orders.Where(filter);
    }


    /// <summary>
    /// Updates entity object if it exists but if it does not, throws an InvalidOperationException.
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Update(Order item)
    {
        var existing = DataSource.Orders.FirstOrDefault(o => o?.Id == item.Id);
        if (existing == null)
            throw new DalDoesNotExistException($"Order with Id {item.Id} does not exist");

        DataSource.Orders.RemoveAll(o => o?.Id == item.Id);
        DataSource.Orders.Add(item);
    }
}
