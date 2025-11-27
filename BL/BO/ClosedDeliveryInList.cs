using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO;
using Helpers;

public class ClosedDeliveryInList
{
    public int DeliveryId { get; set; }
    public int OrderId { get; set; }
    public OrderType OrderType{ get; set; }
    public string AddressOfOrder { get; set; }
    public DeliveryType DeliveryType { get; set; }
    public double? ActualDistance { get; set; }
    public TimeSpan? ActualTimeOfCompletion { get; set; }
    public TypeOfDeliveryCompletionTime TypeOfDeliveryCompletionTime { get; set; }

    public override string ToString() => this.ToStringProperty();
}
