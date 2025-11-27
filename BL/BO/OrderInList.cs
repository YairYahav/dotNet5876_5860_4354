
namespace BO;
using Helpers;
public class OrderInList
{
    public int DeliveryId { get; set; }
    public int OrderId { get; set; }
    public OrderType OrderType{ get; set; }
    public double AirDistance { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public ScheduleStatus ScheduleStatus { get; set; }
    public TimeSpan RemainingTineToCompletion { get; set; }
    public TimeSpan ExpectedTimeToCompletion { get; set; }
    public int AmountOfDeliveries { get; set; }
    

}
