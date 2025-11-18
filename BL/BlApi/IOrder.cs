namespace BlApi;
using BO;
using System.Collections.Generic;
using System;

public interface IOrder
{
    void Create(BO.Order boOrder);
    BO.Order Read(int id);
    IEnumerable<BO.Order> ReadAll(Func<BO.Order, bool>? filter = null);
    void Update(BO.Order boOrder);
    void Delete(int id);
}