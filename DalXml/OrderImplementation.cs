namespace Dal;
using DalApi;
using DO;


internal class OrderImplementation : IOrder
{
    /// <summary>
    /// Creates a new order and saves it to the XML file.
    /// </summary>
    /// <param name="item">The order object to be added.</param>
    /// <exception cref="DalAlreadyExistsException">
    /// Thrown when an order with the same Id already exists in the XML file.
    /// </exception>
    /// <remarks>
    /// Execution steps:
    /// 1. Loads the current list of orders from the XML file.
    /// 2. If the given order has an Id value of 0, a new running Id is assigned from the configuration file (data-config.xml).
    /// 3. Checks whether an order with the same Id already exists in the list;
    ///    if so, throws a DalAlreadyExistsException.
    /// 4. Adds the new order to the in-memory list.
    /// 5. Saves the updated list back to the XML file.
    /// </remarks>

    public void Create(Order item)
    {
        var orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_orders_xml);

        Order newOrder = item;
        if (item.Id == 0)
            newOrder = item with { Id = Config.NextOrderId };

        if (orders.Any(o => o.Id == newOrder.Id))
            throw new DalAlreadyExistsException($"Order with Id {newOrder.Id} already exists");

        orders.Add(newOrder);
        XMLTools.SaveListToXMLSerializer(orders, Config.s_orders_xml);
    }

    /// <summary>
    /// Deletes an order with the specified identifier from the data store.
    /// </summary>
    /// <param name="id">The unique identifier of the order to delete.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if no order with the specified <paramref name="id"/> exists in the data store.</exception>
    public void Delete(int id)
    {
        List<Order> orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_orders_xml);
        if(orders.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Order with Id {id} does not exist");
        XMLTools.SaveListToXMLSerializer<Order>(orders, Config.s_orders_xml);
    }

    /// <summary>
    /// Deletes all orders from the data store.
    /// </summary>
    /// <remarks>This method clears the entire list of orders and saves an empty list to the data store. Use
    /// this method with caution, as it will irreversibly remove all existing orders.</remarks>
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer<Order>(new List<Order>(), Config.s_orders_xml);    
    }

    /// <summary>
    /// Retrieves an order by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the order to retrieve.</param>
    /// <returns>The <see cref="Order"/> object with the specified identifier.</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if no order with the specified <paramref name="id"/> exists.</exception>
    public Order? Read(int id)
    {
        var list = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_orders_xml);
        var order = list.FirstOrDefault(it => it.Id == id);
        //if (order == null)
        //    throw new DalDoesNotExistException($"Courier with Id {id} does not exist");
        return order;
    }

    /// <summary>
    /// Retrieves the first order that matches the specified filter criteria.
    /// </summary>
    /// <param name="filter">A function that defines the conditions an order must satisfy to be returned. Cannot be <see langword="null"/>.</param>
    /// <returns>The first <see cref="Order"/> that matches the filter criteria, or <see langword="null"/> if no matching order
    /// is found.</returns>
    /// <exception cref="DalNullReferenceException">Thrown if <paramref name="filter"/> is <see langword="null"/>.</exception>
    /// <exception cref="DalDoesNotExistException">Thrown if no order matches the specified filter criteria.</exception>
    public Order? Read(Func<Order, bool> filter)
    {
        if(filter == null)
            throw new DalNullReferenceException("Filter function cannot be null");
        List<Order> orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_orders_xml);
        Order? order = orders.FirstOrDefault(filter);
        if (order == null)
            throw new DalDoesNotExistException($"No order match the filter");
        return order;
    }

    public IEnumerable<Order> ReadAll(Func<Order, bool>? filter = null)
    {
        return filter == null ?
            XMLTools.LoadListFromXMLSerializer<Order>(Config.s_orders_xml) :
            XMLTools.LoadListFromXMLSerializer<Order>(Config.s_orders_xml).Where(filter);
    }

    /// <summary>
    /// Updates an existing order in the data store.
    /// </summary>
    /// <remarks>This method replaces the existing order with the same ID as the provided <paramref
    /// name="item"/>. If no matching order is found, an exception is thrown. The updated list of orders is saved to the
    /// XML data store.</remarks>
    /// <param name="item">The <see cref="Order"/> object containing the updated details. The order must have a valid and existing ID.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if no order with the same ID as <paramref name="item"/> exists in the data store.</exception>
    public void Update(Order item)
    {
        List<Order> orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_orders_xml);
        if(orders.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Order with Id {item.Id} does not exist");
        orders.Add(item);
        XMLTools.SaveListToXMLSerializer<Order>(orders, Config.s_orders_xml);
    }
}

