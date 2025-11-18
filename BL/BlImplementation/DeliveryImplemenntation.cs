namespace BlImplementation;
using BlApi;
using BO;
using System;
using System.Collections.Generic;
using Helpers;

internal class DeliveryImplementation : IDelivery
{
    public void Create(Delivery boDelivery)
    {
        throw new NotImplementedException("Creation logic not yet implemented in BL.");
    }

    public Delivery Read(int id)
    {
        throw new NotImplementedException("Read logic not yet implemented in BL.");
    }

    public IEnumerable<Delivery> ReadAll(Func<Delivery, bool>? filter = null)
    {
        return new List<Delivery>();
    }

    public void Update(Delivery boDelivery)
    {
        throw new NotImplementedException("Update logic not yet implemented in BL.");
    }

    public void Delete(int id)
    {
        throw new NotImplementedException("Delete logic not yet implemented in BL.");
    }
}