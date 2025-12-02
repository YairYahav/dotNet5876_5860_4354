using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO;
using Helpers;

public class ClosedDeliveryInList
{
    public int DeliveryId { get; init; }
    public int OrderId { get; init; }
    public OrderType OrderType{ get; init; }
    public string AddressOfOrder { get; init; }
    public DeliveryType DeliveryType { get; init; }
    public double? ActualDistance { get; init; }
    public TimeSpan? ActualTimeOfCompletion { get; init; }
    public TypeOfDeliveryCompletionTime TypeOfDeliveryCompletionTime { get; init; }

    public override string ToString() => this.ToStringProperty();
}
