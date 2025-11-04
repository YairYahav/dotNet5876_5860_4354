using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalApi
{
    public interface IDal
    {
        IConfig Config { get; }

        ICourier Courier { get; }

        IOrder Order { get; }

        IDelivery Delivery { get; }

        void ResetDB();
    }
}
