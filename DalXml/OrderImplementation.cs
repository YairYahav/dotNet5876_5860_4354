namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

internal class OrderImplementation : IOrder
{
    public void Create(Order item)
    {
        List<Order> orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_orders_xml);
        if(orders.FirstOrDefault(it => it.Id == item.Id) != null)
            throw new DalAlreadyExistsException($"Order with Id {item.Id} already exists");
        orders.Add(item);
        XMLTools.SaveListToXMLSerializer<Order>(orders, Config.s_orders_xml);
    }

    public void Delete(int id)
    {
        List<Order> orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_orders_xml);
        if(orders.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Order with Id {id} does not exist");
        XMLTools.SaveListToXMLSerializer<Order>(orders, Config.s_orders_xml);
    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer<Order>(new List<Order>(), Config.s_orders_xml);    
    }

    public Order? Read(int id)
    {
        List<Order> orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_orders_xml);
        Order? order = orders.FirstOrDefault(it => it.Id == id);
        if (order == null)
            throw new DalDoesNotExistException($"Order with Id {id} does not exist");
        return order;
    }

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

    public void Update(Order item)
    {
        List<Order> orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_orders_xml);
        if(orders.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Order with Id {item.Id} does not exist");
        orders.Add(item);
        XMLTools.SaveListToXMLSerializer<Order>(orders, Config.s_orders_xml);
    }
}

