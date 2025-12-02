
namespace BO;
using Helpers;

public class DeliveryPerOrderInList
{
    public int DeliveryId { get; init; }
    public int? CourierId { get; init; }
    public string CourierName { get; init; } 
    public DeliveryType DeliveryType { get; init; }
    public DateTime DeliveryStartTime { get; init; }
    public TypeOfDeliveryCompletionTime TypeOfDeliveryCompletionTime { get; init; }
    public DateTime DeliveryEndTime { get; init; }

    public override string ToString() => this.ToStringProperty();

}
