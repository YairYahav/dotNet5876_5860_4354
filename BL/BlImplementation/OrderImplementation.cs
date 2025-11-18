namespace BlImplementation;
using BlApi;
using BO;
using System;
using System.Collections.Generic;
using Helpers;

internal class OrderImplementation : IOrder
{
    public void Create(Order boOrder)
    {
        throw new NotImplementedException("Creation logic not yet implemented in BL.");
    }

    public Order Read(int id)
    {
        throw new NotImplementedException("Read logic not yet implemented in BL.");
    }

    public IEnumerable<Order> ReadAll(Func<Order, bool>? filter = null)
    {
        return new List<Order>();
    }

    public void Update(Order boOrder)
    {
        throw new NotImplementedException("Update logic not yet implemented in BL.");
    }

    public void Delete(int id)
    {
        throw new NotImplementedException("Delete logic not yet implemented in BL.");
    }
}