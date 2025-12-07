using Helpers;

namespace BO;

/// <summary>
/// Represents an order that is currently in progress with a courier.
/// Contains comprehensive delivery tracking information and status metrics.
/// </summary>
/// <remarks>
/// This class encapsulates all details about an active delivery, including order information,
/// delivery metrics, timing information, and current status. It is used to track orders
/// that are actively being delivered by a courier.
/// </remarks>
public class OrderInProgress
{
    /// <summary>
    /// Gets the unique identifier for the delivery.
    /// </summary>
    /// <value>An integer representing the delivery ID.</value>
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
    public OrderType OrderType { get; init; }

    /// <summary>
    /// Gets the description of the order.
    /// </summary>
    /// <value>A text description of the order contents or special instructions, or null if not provided.</value>
    public string? DescriptionOfOrder { get; init; }

    /// <summary>
    /// Gets the delivery address for the order.
    /// </summary>
    /// <value>The physical address where the order is being delivered.</value>
    public string AddressOfOrder { get; init; }

    /// <summary>
    /// Gets the air distance from the company to the delivery location in kilometers.
    /// </summary>
    /// <value>The straight-line distance in kilometers.</value>
    public double AirDistance { get; init; }

    /// <summary>
    /// Gets the actual distance being traveled for this delivery in kilometers.
    /// </summary>
    /// <value>The actual travel distance in kilometers, or null if not calculated.</value>
    public double? ActualDistance { get; init; }

    /// <summary>
    /// Gets the name of the customer receiving the order.
    /// </summary>
    /// <value>The customer's full name.</value>
    public string CustomerName { get; init; }

    /// <summary>
    /// Gets the phone number of the customer.
    /// </summary>
    /// <value>The customer's contact phone number.</value>
    public string CustomerPhone { get; init; }

    /// <summary>
    /// Gets the date and time when the order was placed.
    /// </summary>
    /// <value>A DateTime representing when the order was created.</value>
    public DateTime OrderPlacementTime { get; init; }

    /// <summary>
    /// Gets the date and time when the courier picked up the order.
    /// </summary>
    /// <value>A DateTime representing when the pickup occurred.</value>
    public DateTime PickUpTime { get; init; }

    /// <summary>
    /// Gets the expected delivery time based on the delivery type and distance.
    /// </summary>
    /// <value>A DateTime representing the expected completion time.</value>
    public DateTime DeliveryTime { get; init; }

    /// <summary>
    /// Gets the maximum allowed delivery time for the order.
    /// </summary>
    /// <value>A DateTime representing the deadline for delivery.</value>
    public DateTime MaxDelivryTime { get; init; }

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
    /// Gets the remaining time until the order must be delivered.
    /// </summary>
    /// <value>A TimeSpan representing the time remaining for completion.</value>
    public TimeSpan TimeLeftToDelivery { get; init; }

    /// <summary>
    /// Returns a string representation of the order in progress.
    /// </summary>
    /// <returns>A formatted string containing the order's properties.</returns>
    public override string ToString() => this.ToStringProperty();
}
