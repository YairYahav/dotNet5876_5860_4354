namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

internal class CourierImplementation : ICourier
{
    public void Create(Courier item)
    {
        List<Courier> couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml);
        Courier? courier = couriers.FirstOrDefault(it => it.Id == item.Id);
        if (courier != null)
            throw new DalAlreadyExistsException($"Courier with Id {item.Id} already exists");   
        couriers.Add(item);
        XMLTools.SaveListToXMLSerializer<Courier>(couriers, Config.s_couriers_xml);
    }

    public void Delete(int id)
    {
        List<Courier> couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml);
        if(couriers.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Courier with Id {id} does not exist");
        XMLTools.SaveListToXMLSerializer<Courier>(couriers, Config.s_couriers_xml);

    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer<Courier>(new List<Courier>(), Config.s_couriers_xml);
    }

    public Courier? Read(int id)
    {
        List<Courier> couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml);
        Courier? courier = couriers.FirstOrDefault(it => it.Id == id);
        if (courier == null)
            throw new DalDoesNotExistException($"Courier with Id {id} does not exist");
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

    public void Update(Courier item)
    {
        List<Courier> couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml);
        if(couriers.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Courier with Id {item.Id} does not exist");
        couriers.Add(item);
        XMLTools.SaveListToXMLSerializer<Courier>(couriers, Config.s_couriers_xml);
    }
}
