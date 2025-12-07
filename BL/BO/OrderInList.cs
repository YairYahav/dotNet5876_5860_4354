namespace BO;
using Helpers;

/// <summary>
/// Represents an order's information in a list view format.
/// Contains order summary details including status, scheduling, and delivery metrics.
/// </summary>
/// <remarks>
/// This class is used to display order information in lists or collections,
/// providing a view-optimized representation with order status, scheduling information,
/// and expected delivery metrics.
/// </remarks>
public class OrderInList
{
    /// <summary>
    /// Gets the unique identifier for the delivery associated with this order.
    /// </summary>
    /// <value>An integer representing the delivery ID, or 0 if no delivery is assigned.</value>
    public int DeliveryId { get; init; }

    /// <summary>
    /// Gets the unique identifier for the order.
    /// </summary>
    /// <value>A unique integer identifier assigned to the order.</value>
    public int OrderId { get; init; }

    /// <summary>
    /// Gets the type of order (Regular, Express, Heavy, Fragile, Refrigerated).
    /// </summary>
    /// <value>An OrderType enumeration value specifying the order category.</value>
    public OrderType OrderType{ get; init; }

    /// <summary>
    /// Gets the air distance from the company to the delivery location in kilometers.
    /// </summary>
    /// <value>The straight-line distance in kilometers.</value>
    public double AirDistance { get; init; }

    /// <summary>
    /// Gets the current status of the order (Open, InProgress, Delivered, etc.).
    /// </summary>
    /// <value>An OrderStatus enumeration value.</value>
    public OrderStatus OrderStatus { get; init; }

    /// <summary>
    /// Gets the scheduling status of the order (OnTime, Late, InRisk).
    /// </summary>
    /// <value>A ScheduleStatus enumeration value.</value>
    public ScheduleStatus ScheduleStatus { get; init; }

    /// <summary>
    /// Gets the remaining time until the order must be completed.
    /// </summary>
    /// <value>A TimeSpan representing the time remaining for completion.</value>
    public TimeSpan RemainingTineToCompletion { get; init; }

    /// <summary>
    /// Gets the expected time required to complete the delivery.
    /// </summary>
    /// <value>A TimeSpan representing the estimated delivery duration.</value>
    public TimeSpan ExpectedTimeToCompletion { get; init; }

    /// <summary>
    /// Gets the number of deliveries associated with this order.
    /// </summary>
    /// <value>An integer representing the count of delivery attempts.</value>
    public int AmountOfDeliveries { get; init; }

    /// <summary>
    /// Returns a string representation of the order list item.
    /// </summary>
    /// <returns>A formatted string containing the order's properties.</returns>
    public override string ToString() => this.ToStringProperty();
}
