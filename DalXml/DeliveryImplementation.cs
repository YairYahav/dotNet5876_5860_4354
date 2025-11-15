namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

internal class DeliveryImplementation : IDelivery
{
    public void Create(Delivery item)
    {
        List<Delivery> deliveries = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml);
        Delivery? delivery = deliveries.FirstOrDefault(it => it.Id == item.Id);
        if (delivery != null)
            throw new DalAlreadyExistsException($"Delivery with Id {item.Id} already exists");
        deliveries.Add(item);
        XMLTools.SaveListToXMLSerializer<Delivery>(deliveries, Config.s_deliveries_xml);
    }

    public void Delete(int id)
    {
        List<Delivery> deliveries = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml);
        if (deliveries.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Delivery with Id {id} does not exist");
        XMLTools.SaveListToXMLSerializer<Delivery>(deliveries, Config.s_deliveries_xml);
    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer<Delivery>(new List<Delivery>(), Config.s_deliveries_xml);
    }

    public Delivery? Read(int id)
    {
        List<Delivery> deliveries = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml);
        Delivery? delivery = deliveries.FirstOrDefault(it => it.Id == id);
        if (delivery == null)
            throw new DalDoesNotExistException($"Delivery with Id {id} does not exist");
        return delivery;
    }

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

    public void Update(Delivery item)
    {
        List<Delivery> deliveries = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml);
        if(deliveries.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Delivery with Id {item.Id} does not exist");
        deliveries.Add(item);
        XMLTools.SaveListToXMLSerializer<Delivery>(deliveries, Config.s_deliveries_xml);
    }
}
