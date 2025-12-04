using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL;

public class DeliveryTypeCollection : IEnumerable
{
    static readonly IEnumerable<BO.DeliveryType> s_enums =
        (Enum.GetValues(typeof(BO.DeliveryType)) as IEnumerable<BO.DeliveryType>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
