
namespace BO;
using Helpers;

public class DeliveryPerOrderInList
{
    public int DeliveryId { get; set; }
    public int? CourierId { get; set; }
    public string CourierName { get; set; } 
    public DeliveryType DeliveryType { get; set; }
    public DateTime DeliveryStartTime { get; set; }
    public TypeOfDeliveryCompletionTime TypeOfDeliveryCompletionTime { get; set; }
    public DateTime DeliveryEndTime { get; set; }

    public override string ToString() => this.ToStringProperty();

}
