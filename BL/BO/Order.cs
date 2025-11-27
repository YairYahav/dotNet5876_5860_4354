
namespace BO;
using Helpers;

public class Order
{
    public int Id { get; set; }
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
    public DateTime? OrderPlacementTime { get; set; }
    public DateTime? ExpectedCompletionTime { get; set; }
    public DateTime MaxDeliveryTime { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public ScheduleStatus ScheduleStatus { get; set; }
    public DateTime RemainingTimeToDelivery { get; set; }
    public DeliveryPerOrderInList? DeliveryPerOrderInList { get; set; }

    public override string ToString() => this.ToStringProperty();

}
