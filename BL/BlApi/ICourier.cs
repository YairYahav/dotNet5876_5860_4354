namespace BlApi;
using BO;
using System.Collections.Generic;
using System;

public interface ICourier
{
    void Create(BO.Courier boCourier);
    BO.Courier Read(int id);
    IEnumerable<BO.Courier> ReadAll(Func<BO.Courier, bool>? filter = null);
    void Update(BO.Courier boCourier);
    void Delete(int id);
}