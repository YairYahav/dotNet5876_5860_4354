namespace Dal;
using DalApi;
using DO;
internal class DeliveryImplementation : IDelivery
{

    /// <summary>
    /// Creates a new delivery and saves it to the XML file.
    /// </summary>
    /// <param name="item">The delivery object to be added.</param>
    /// <exception cref="DalAlreadyExistsException">
    /// Thrown when a delivery with the same Id already exists in the XML file.
    /// </exception>
    /// <remarks>
    /// Execution steps:
    /// 1. Loads the current list of deliveries from the XML file.
    /// 2. If the given delivery has an Id value of 0, a new running Id is assigned from the configuration file (data-config.xml).
    /// 3. Checks whether a delivery with the same Id already exists in the list;
    ///    if so, throws a DalAlreadyExistsException.
    /// 4. Adds the new delivery to the in-memory list.
    /// 5. Saves the updated list back to the XML file.
    /// </remarks>
    public void Create(Delivery item)
    {
        List<Delivery> deliveries = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml);
        
        Delivery newDelivery = item;
        if (item.Id == 0)
            newDelivery = item with { Id = Config.NextDeliveryId };
        
        if (deliveries.Any(d => d.Id == newDelivery.Id))
            throw new DalAlreadyExistsException($"Delivery with Id {newDelivery.Id} already exists");
        
        deliveries.Add(newDelivery);
        XMLTools.SaveListToXMLSerializer<Delivery>(deliveries, Config.s_deliveries_xml);
    }

    /// <summary>
    /// Deletes a delivery with the specified identifier.
    /// </summary>
    /// <remarks>This method removes the delivery from the underlying data store. If the specified <paramref
    /// name="id"/> does not match any existing delivery, an exception is thrown.</remarks>
    /// <param name="id">The unique identifier of the delivery to delete.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if no delivery with the specified <paramref name="id"/> exists.</exception>
    public void Delete(int id)
    {
        List<Delivery> deliveries = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml);
        if (deliveries.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Delivery with Id {id} does not exist");
        XMLTools.SaveListToXMLSerializer<Delivery>(deliveries, Config.s_deliveries_xml);
    }
    /// <summary>
    /// Deletes all delivery records by clearing the delivery list and saving an empty list to the data source.
    /// </summary>
    /// <remarks>This method resets the delivery data to an empty state. It overwrites the existing data
    /// source with an empty list. Use this method with caution, as all existing delivery records will be permanently
    /// removed.</remarks>
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer<Delivery>(new List<Delivery>(), Config.s_deliveries_xml);
    }

    /// <summary>
    /// Retrieves a delivery by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the delivery to retrieve.</param>
    /// <returns>The <see cref="Delivery"/> object with the specified identifier.</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if no delivery with the specified <paramref name="id"/> exists.</exception>
    public Delivery? Read(int id)
    {
        var list = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml);
        var delivery = list.FirstOrDefault(it => it.Id == id);
        //if (delivery == null)
        //    throw new DalDoesNotExistException($"Courier with Id {id} does not exist");
        return delivery;
    }

    /// <summary>
    /// Retrieves the first <see cref="Delivery"/> object that matches the specified filter criteria.
    /// </summary>
    /// <param name="filter">A function that defines the conditions of the <see cref="Delivery"/> to retrieve. Cannot be <see
    /// langword="null"/>.</param>
    /// <returns>The first <see cref="Delivery"/> object that satisfies the filter criteria.</returns>
    /// <exception cref="DalNullReferenceException">Thrown if <paramref name="filter"/> is <see langword="null"/>.</exception>
    /// <exception cref="DalDoesNotExistException">Thrown if no <see cref="Delivery"/> matches the specified filter criteria.</exception>
    public Delivery? Read(Func<Delivery, bool> filter)
    {
        if (filter == null)
            throw new DalNullReferenceException("Filter function cannot be null");
        List<Delivery> deliveries = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml);
        Delivery? delivery = deliveries.FirstOrDefault(filter);
        if (delivery == null)
            throw new DalDoesNotExistException($"No delivery match the filter");
        return delivery;
    }


    public IEnumerable<Delivery> ReadAll(Func<Delivery, bool>? filter = null)
    {
        return filter == null ?
            XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml) :
            XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml).Where(filter);
    }

    /// <summary>
    /// Updates the specified delivery in the data store.
    /// </summary>
    /// <remarks>This method replaces the existing delivery with the same <see cref="Delivery.Id"/> as the
    /// specified <paramref name="item"/>. If no matching delivery is found, an exception is thrown. The updated list of
    /// deliveries is persisted to the data store.</remarks>
    /// <param name="item">The <see cref="Delivery"/> object containing the updated information. The delivery must have a valid and
    /// existing <see cref="Delivery.Id"/>.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if a delivery with the same <see cref="Delivery.Id"/> as the specified <paramref name="item"/> does not
    /// exist in the data store.</exception>
    public void Update(Delivery item)
    {
        List<Delivery> deliveries = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml);
        if(deliveries.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Delivery with Id {item.Id} does not exist");
        deliveries.Add(item);
        XMLTools.SaveListToXMLSerializer<Delivery>(deliveries, Config.s_deliveries_xml);
    }
}
