namespace BO;
using Helpers;

/// <summary>
/// Represents a closed delivery entry in a list, containing details about the delivery and its completion.
/// </summary>
/// <remarks>This class provides information about a completed delivery, including identifiers, order type,
/// delivery type, and metrics such as actual distance and time of completion. It is used to encapsulate delivery
/// details for reporting or tracking purposes.</remarks>
public class ClosedDeliveryInList
{
    /// <summary>
    /// Gets the unique identifier for the delivery.
    /// </summary>
    /// <value>An integer representing the delivery ID.</value>
    public int DeliveryId { get; init; }

    /// <summary>
    /// Gets the unique identifier for the order associated with this delivery.
    /// </summary>
    /// <value>An integer representing the order ID.</value>
    public int OrderId { get; init; }

    /// <summary>
    /// Gets the type of order (Regular, Express, Heavy, Fragile, Refrigerated).
    /// </summary>
    /// <value>An OrderType enumeration value specifying the order category.</value>
    public OrderType OrderType{ get; init; }

    /// <summary>
    /// Gets the delivery address for this order.
    /// </summary>
    /// <value>The physical address where the order was delivered.</value>
    public string AddressOfOrder { get; init; }

    /// <summary>
    /// Gets the type of delivery used (Car, Motorcycle, Bicycle, OnFoot).
    /// </summary>
    /// <value>A DeliveryType enumeration value.</value>
    public DeliveryType DeliveryType { get; init; }

    /// <summary>
    /// Gets the actual distance traveled for this delivery in kilometers.
    /// </summary>
    /// <value>The distance in kilometers, or null if not calculated.</value>
    public double? ActualDistance { get; init; }

    /// <summary>
    /// Gets the actual time taken to complete the delivery.
    /// </summary>
    /// <value>A TimeSpan representing the duration from pickup to completion, or null if not completed.</value>
    public TimeSpan? ActualTimeOfCompletion { get; init; }

    /// <summary>
    /// Gets the type of delivery completion (Supplied, Canceled, Failed, RefusedByCustomer, CustomerNotFound).
    /// </summary>
    /// <value>A TypeOfDeliveryCompletionTime enumeration value.</value>
    public TypeOfDeliveryCompletionTime TypeOfDeliveryCompletionTime { get; init; }

    /// <summary>
    /// Returns a string representation of the closed delivery list item.
    /// </summary>
    /// <returns>A formatted string containing the delivery's properties.</returns>
    public override string ToString() => this.ToStringProperty();
}
