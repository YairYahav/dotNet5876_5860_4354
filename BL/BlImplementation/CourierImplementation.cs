namespace BlImplementation;
using BlApi;
using BO;
using System;
using System.Collections.Generic;
using Helpers;

internal class CourierImplementation : ICourier
{
    public void Create(Courier boCourier)
    {
        throw new NotImplementedException("Creation logic not yet implemented in BL.");
    }

    public Courier Read(int id)
    {
        throw new NotImplementedException("Read logic not yet implemented in BL.");
    }

    public IEnumerable<Courier> ReadAll(Func<Courier, bool>? filter = null)
    {
        return new List<Courier>();
    }

    public void Update(Courier boCourier)
    {
        throw new NotImplementedException("Update logic not yet implemented in BL.");
    }

    public void Delete(int id)
    {
        throw new NotImplementedException("Delete logic not yet implemented in BL.");
    }
}