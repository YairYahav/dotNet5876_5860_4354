
namespace Dal;
using DalApi;
using DO;
using System.Linq;

/// <summary>
/// Implementation of the ICourier interface for managing Courier entities in the Data Access Layer (DAL).  
/// </summary>
internal class CourierImplementation : ICourier
{
    /// <summary>
    /// Creates new entity object in DAL if it does not already exist but if it does, throws an InvalidOperationException.
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Create(Courier item) 
    {
        if (Read(item.Id) != null)
            throw new DalAlreadyExistsException($"Courier with Id {item.Id} already exists");
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
            throw new DalDoesNotExistException($"Courier with Id {id} does not exist");
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
        var courier = DataSource.Couriers.FirstOrDefault(c => c?.Id == id);
        //if (courier == null)
        //    throw new DalDoesNotExistException($"Courier with ID {id} does not exist");
        return courier;
    }


    public Courier? Read(Func<Courier, bool> filter)
    {
        if (filter == null) throw new DalNullReferenceException("Filter function cannot be null");
        // אפשר על אותו עיקרון במקום להחזיר שגיאה פשוט להחזיר נל בדרך דומה למה שעשיתי ב-8ג
        // filter == null ? null : DataSource.Couriers.FirstOrDefault(filter);

        var courier = DataSource.Couriers.FirstOrDefault(filter);
        if (courier == null)
            throw new DalDoesNotExistException($"No courier match the filter");
        return courier;
    }


    /// <summary>
    /// Reads all entity objects.
    /// </summary>
    /// <returns> The list of all Courier objects. </returns>
    public IEnumerable<Courier> ReadAll(Func<Courier, bool>? filter = null) // Stage 2
    {
        // אכתוב הסבר לדרך כתיבה הזאת שאני מכיר מקורס אחר
        // condition ? (if true) : (else)
        return filter == null
            ? DataSource.Couriers.Select(item => item) // logical copy נחזיר עותק לוגי, לא חובה אבל זה ההסבר לפנים הסוגריים
            : DataSource.Couriers.Where(filter);
    }

    /// <summary>
    /// Updates entity object if it exists but if it does not, throws an InvalidOperationException.
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Update(Courier item)
    {
        var existing = DataSource.Couriers.FirstOrDefault(c => c?.Id == item.Id);
        if (existing == null)
            throw new DalDoesNotExistException($"Courier with ID {item.Id} does not exist");

        DataSource.Couriers.RemoveAll(c => c?.Id == item.Id);
        DataSource.Couriers.Add(item);
    }
}
