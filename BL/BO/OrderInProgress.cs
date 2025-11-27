using Helpers;

namespace BO;

public class OrderInProgress
{
    public int DeliveryId { get; init; }
    public int OrderId { get; init; }
    public OrderType OrderType { get; set; }
    public string? DescriptionOfOrder { get; set; }
    public string AddressOfOrder { get; set; }
    public double AirDistance { get; set; }
    public double? ActualDistance { get; set; }
    public string CustomerName { get; set; }
    public string CustomerPhone { get; set; }
    public DateTime OrderPlacementTime { get; set; }
    public DateTime PickUpTime { get; set; }
    public DateTime DeliveryTime { get; set; }
    public DateTime MaxDelivryTime { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public ScheduleStatus ScheduleStatus { get; set; }
    public TimeSpan TimeLeftToDelivery { get; set; }

    public override string ToString() => this.ToStringProperty();



}
