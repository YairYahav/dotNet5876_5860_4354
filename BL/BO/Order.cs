
namespace BO;
using Helpers;

public class Order
{
    public int Id { get; init; }
    public OrderType orderType { get; set; }
    public string? DescriptionOfOrder { get; set; }
    public string AddressOfOrder { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double AirDistance { get; set; }
    public string CustomerName { get; set; }
    public string CustomerPhone { get; set; }
    public bool IsFrag { get; set; }
    public double? Volume { get; set; }
    public double? Weight { get; set; }
    public DateTime? OrderPlacementTime { get; init; }
    public DateTime? ExpectedCompletionTime { get; init; }
    public DateTime MaxDeliveryTime { get; init; }
    public OrderStatus OrderStatus { get; init; }
    public ScheduleStatus ScheduleStatus { get; init; }
    public DateTime RemainingTimeToDelivery { get; init; }
    public DeliveryPerOrderInList? DeliveryPerOrderInList { get; init; }

    public override string ToString() => this.ToStringProperty();

}
