using Helpers;

namespace BO;

public class OrderInProgress
{
    public int DeliveryId { get; init; }
    public int OrderId { get; init; }
    public OrderType OrderType { get; init; }
    public string? DescriptionOfOrder { get; init; }
    public string AddressOfOrder { get; init; }
    public double AirDistance { get; init; }
    public double? ActualDistance { get; init; }
    public string CustomerName { get; init; }
    public string CustomerPhone { get; init; }
    public DateTime OrderPlacementTime { get; init; }
    public DateTime PickUpTime { get; init; }
    public DateTime DeliveryTime { get; init; }
    public DateTime MaxDelivryTime { get; init; }
    public OrderStatus OrderStatus { get; init; }
    public ScheduleStatus ScheduleStatus { get; init; }
    public TimeSpan TimeLeftToDelivery { get; init; }

    public override string ToString() => this.ToStringProperty();



}
