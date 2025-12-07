using Helpers;

namespace BO;

/// <summary>
/// Represents an open order that is available for assignment to a courier.
/// Contains order details and delivery availability metrics for matching with suitable couriers.
/// </summary>
/// <remarks>
/// This class is used to display orders that are available for delivery, providing
/// courier-relevant information such as delivery range, time constraints, and package specifications.
/// It helps match orders with appropriate couriers based on their capabilities and location.
/// </remarks>
public class OpenOrderInList
{
    /// <summary>
    /// Gets the unique identifier for the courier assigned to this order, if any.
    /// </summary>
    /// <value>An integer representing the courier ID, or null if not yet assigned.</value>
    public int? CourierId { get; init; }

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
    /// Gets a value indicating whether the order contains fragile items.
    /// </summary>
    /// <value>True if the order is fragile; otherwise, false.</value>
    public bool IsFrag { get; init; }

    /// <summary>
    /// Gets the weight of the order in kilograms.
    /// </summary>
    /// <value>The weight measurement, or null if not specified.</value>
    public double? Weight { get; init; }

    /// <summary>
    /// Gets the volume of the order in cubic units.
    /// </summary>
    /// <value>The volume measurement, or null if not specified.</value>
    public double? Volume { get; init; }

    /// <summary>
    /// Gets the delivery address for the order.
    /// </summary>
    /// <value>The physical address where the order should be delivered.</value>
    public string AddressOfOrder { get; init; }

    /// <summary>
    /// Gets the air distance from the company to the delivery location in kilometers.
    /// </summary>
    /// <value>The straight-line distance in kilometers.</value>
    public double AirDistance { get; init; }

    /// <summary>
    /// Gets the actual distance for the delivery based on the selected delivery type.
    /// </summary>
    /// <value>The actual travel distance in kilometers, or null if not calculated.</value>
    public double? ActualDistance { get; init; }

    /// <summary>
    /// Gets the actual time taken to complete the delivery.
    /// </summary>
    /// <value>A TimeSpan representing the delivery duration, or null if not calculated.</value>
    public TimeSpan? ActualTime { get; init; }

    /// <summary>
    /// Gets the scheduling status of the order (OnTime, Late, InRisk).
    /// </summary>
    /// <value>A ScheduleStatus enumeration value.</value>
    public ScheduleStatus ScheduleStatus { get; init; }

    /// <summary>
    /// Gets the remaining time until the order must be completed.
    /// </summary>
    /// <value>A TimeSpan representing the time remaining for completion.</value>
    public TimeSpan RemainingTimeForCompletion { get; init; }

    /// <summary>
    /// Gets the maximum allowed delivery time for the order.
    /// </summary>
    /// <value>A DateTime representing the deadline for delivery.</value>
    public DateTime MaxDeliveryTime { get; init; }

    /// <summary>
    /// Returns a string representation of the open order list item.
    /// </summary>
    /// <returns>A formatted string containing the order's properties.</returns>
    public override string ToString() => this.ToStringProperty();
}
