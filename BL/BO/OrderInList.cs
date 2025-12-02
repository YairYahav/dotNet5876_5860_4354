
namespace BO;
using Helpers;
public class OrderInList
{
    public int DeliveryId { get; init; }
    public int OrderId { get; init; }
    public OrderType OrderType{ get; init; }
    public double AirDistance { get; init; }
    public OrderStatus OrderStatus { get; init; }
    public ScheduleStatus ScheduleStatus { get; init; }
    public TimeSpan RemainingTineToCompletion { get; init; }
    public TimeSpan ExpectedTimeToCompletion { get; init; }
    public int AmountOfDeliveries { get; init; }
    

}
