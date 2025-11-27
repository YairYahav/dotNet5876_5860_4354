using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO;

public class OpenOrderInList
{
    public int? CourierId { get; set; }
    public int OrderId { get; set; }
    public OrderType OrderType { get; set; }
    public bool IsFrag { get; set; }
    public double? Weight { get; set; }
    public double? Volume { get; set; }
    public string AddressOfOrder { get; set; }
    public double AirDistance { get; set; }
    public double? ActualDistance { get; set; }
    public TimeSpan? ActualTime { get; set; }
    public ScheduleStatus ScheduleStatus { get; set; }
    public TimeSpan RemainingTimeForCompletion { get; set; }
    public DateTime MaxDeliveryTime { get; set; }

    public override string ToString() => this.ToStringProperty();

}
