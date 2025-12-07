namespace BO;
using Helpers;

/// <summary>
/// Represents delivery information for an order in a list view format.
/// Contains tracking details about a specific delivery attempt or assignment.
/// </summary>
/// <remarks>
/// This class encapsulates delivery-specific information for an order, including the assigned
/// courier, delivery type, timing, and completion status. It is used to provide delivery tracking
/// information within order list contexts.
/// </remarks>
public class DeliveryPerOrderInList
{
    /// <summary>
    /// Gets the unique identifier for the delivery.
    /// </summary>
    /// <value>An integer representing the delivery ID.</value>
    public int DeliveryId { get; init; }

    /// <summary>
    /// Gets the unique identifier for the courier assigned to this delivery.
    /// </summary>
    /// <value>An integer representing the courier ID, or null if not assigned.</value>
    public int? CourierId { get; init; }

    /// <summary>
    /// Gets the name of the courier assigned to this delivery.
    /// </summary>
    /// <value>The courier's full name.</value>
    public string CourierName { get; init; } 

    /// <summary>
    /// Gets the type of delivery method used (Car, Motorcycle, Bicycle, OnFoot).
    /// </summary>
    /// <value>A DeliveryType enumeration value.</value>
    public DeliveryType DeliveryType { get; init; }

    /// <summary>
    /// Gets the date and time when the delivery started.
    /// </summary>
    /// <value>A DateTime representing when the courier picked up the order.</value>
    public DateTime DeliveryStartTime { get; init; }

    /// <summary>
    /// Gets the type of delivery completion (Supplied, Canceled, Failed, RefusedByCustomer, CustomerNotFound).
    /// </summary>
    /// <value>A TypeOfDeliveryCompletionTime enumeration value.</value>
    public TypeOfDeliveryCompletionTime TypeOfDeliveryCompletionTime { get; init; }

    /// <summary>
    /// Gets the date and time when the delivery ended.
    /// </summary>
    /// <value>A DateTime representing when the delivery was completed.</value>
    public DateTime DeliveryEndTime { get; init; }

    /// <summary>
    /// Returns a string representation of the delivery list item.
    /// </summary>
    /// <returns>A formatted string containing the delivery's properties.</returns>
    public override string ToString() => this.ToStringProperty();
}
