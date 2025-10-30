// Module Delivery.cs
namespace DO;
/// <summary>
/// Delivery information.
/// </summary>
/// <param name="Id">The unique identifier for the delivery.</param>
/// <param name="OrderId">The unique identifier for the associated order.</param>
/// <param name="CourierId">The unique identifier for the courier assigned to the delivery.</param>
/// <param name="DeliveryType">The type of delivery.</param>
/// <param name="DeliveryStartTime">The date and time when the delivery started.</param>
/// <param name="ActualDistance">The actual distance covered during the delivery (optional).</param>
/// <param name="DeliveryCompletionTime">The date and time when the delivery was completed (optional).</param>
/// <param name="TypeOfDeliveryCompletionTime">The type of delivery completion time (optional).</param>
public record Delivery
(
    int Id,
    int OrderId,
    int CourierId,
    DateTime DeliveryStartTime,
    DeliveryType DeliveryType,
    double? ActualDistance = null,
    DateTime? DeliveryCompletionTime = null,
    TypeOfDeliveryCompletionTime? TypeOfDeliveryCompletionTime = null
)
{
    /// <summary>
    /// Default constructor 
    /// </summary>
    public Delivery() : this(0, 0, 0, default, default) { }
}
