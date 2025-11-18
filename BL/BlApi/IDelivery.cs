namespace BlApi;
using BO;
using System.Collections.Generic;
using System;

public interface IDelivery
{
    void Create(BO.Delivery boDelivery);
    BO.Delivery Read(int id);
    IEnumerable<BO.Delivery> ReadAll(Func<BO.Delivery, bool>? filter = null);
    void Update(BO.Delivery boDelivery);
    void Delete(int id);
}