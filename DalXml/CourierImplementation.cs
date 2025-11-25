namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

internal class CourierImplementation : ICourier
{

    /// <summary>
    /// Creates a new courier and saves it to the XML file.
    /// </summary>
    /// <param name="item">The courier object to be added.</param>
    /// <exception cref="DalAlreadyExistsException">
    /// Thrown when a courier with the same Id already exists in the XML file.
    /// </exception>
    /// <remarks>
    /// Execution steps:
    /// 1. Loads the current list of couriers from the XML file.
    /// 2. If the given courier has an Id value of 0, a new running Id is assigned from the configuration file (data-config.xml).
    /// 3. Checks whether a courier with the same Id already exists in the list; 
    ///    if so, throws a DalAlreadyExistsException.
    /// 4. Adds the new courier to the in-memory list.
    /// 5. Saves the updated list back to the XML file.
    /// </remarks>

    public void Create(Courier item)
    {
        var list = XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml);

        Courier newCourier = item;
        if (item.Id == 0)
            newCourier = item with { Id = Config.NextCourierId };

        if (list.Any(c => c.Id == newCourier.Id))
            throw new DalAlreadyExistsException($"Courier with Id {newCourier.Id} already exists");

        list.Add(newCourier);
        XMLTools.SaveListToXMLSerializer(list, Config.s_couriers_xml);
    }

    /// <summary>
    ///  Deletes a courier with the specified identifier from the data store.
    /// </summary>
    /// <remarks>This method removes the courier with the given identifier from the underlying XML data store.
    /// If the specified courier does not exist, an exception is thrown.</remarks>
    /// <param name="id">The unique identifier of the courier to delete.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if no courier with the specified <paramref name="id"/> exists in the data store.</exception>
    public void Delete(int id)
    {
        List<Courier> couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml);
        if(couriers.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Courier with Id {id} does not exist");
        XMLTools.SaveListToXMLSerializer<Courier>(couriers, Config.s_couriers_xml);

    }
    /// <summary>
    /// Deletes all couriers from the data store.
    /// </summary>
    /// <remarks>This method clears the entire list of couriers and saves an empty list to the data store. Use
    /// this method with caution as it will result in the loss of all courier data.</remarks>

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer<Courier>(new List<Courier>(), Config.s_couriers_xml);
    }
    

    /// <summary>
    /// Retrieves a courier by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the courier to retrieve.</param>
    /// <returns>The <see cref="Courier"/> object with the specified identifier.</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if no courier with the specified <paramref name="id"/> exists.</exception>
    public Courier? Read(int id)
    {
        var list = XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml);
        var courier = list.FirstOrDefault(it => it.Id == id);
        //if (courier == null)
        //    throw new DalDoesNotExistException($"Courier with Id {id} does not exist");
        return courier;
    }


    public Courier? Read(Func<Courier, bool> filter)
    {
        if (filter == null)
            throw new DalNullReferenceException("Filter function cannot be null");
        List<Courier> couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml);
        Courier? courier = couriers.FirstOrDefault(filter);
        if (courier == null)
            throw new DalDoesNotExistException($"No courier match the filter");
        return courier;
    }

    public IEnumerable<Courier> ReadAll(Func<Courier, bool>? filter = null)
    {
        return filter == null ?
            XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml) :
            XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml).Where(filter);
    }

    /// <summary>
    /// Updates the details of an existing courier in the data store.
    /// </summary>
    /// <remarks>This method locates the courier in the data store by its <c>Id</c> and replaces the existing
    /// entry with the provided <paramref name="item"/>. Changes are persisted to the underlying XML data
    /// store.</remarks>
    /// <param name="item">The <see cref="Courier"/> object containing the updated details. The courier's <c>Id</c> must match an existing
    /// entry.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if a courier with the specified <c>Id</c> does not exist in the data store.</exception>
    public void Update(Courier item)
    {
        var list = XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml);
        int idx = list.FindIndex(c => c.Id == item.Id);
        if (idx < 0)
            throw new DalDoesNotExistException($"Courier with Id {item.Id} does not exist");
        list[idx] = item;
        XMLTools.SaveListToXMLSerializer(list, Config.s_couriers_xml);
    }

}
