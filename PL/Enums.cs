using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL;

public class DeliveryTypeCollection : IEnumerable
{
    static readonly IEnumerable<object> s_enums =
        new List<object> { "All" }
            .Concat(
                (Enum.GetValues(typeof(BO.DeliveryType)) as IEnumerable<BO.DeliveryType>)!
                    .Where(dt => dt != BO.DeliveryType.None)
                    .Cast<object>()
            )
            .ToList();

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
