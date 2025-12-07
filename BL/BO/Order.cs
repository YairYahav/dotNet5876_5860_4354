namespace BO;
using Helpers;

/// <summary>
/// Represents an order entity in the business logic layer.
/// Contains order details, customer information, location data, and delivery status.
/// </summary>
/// <remarks>
/// An order is a customer request for delivery of a package. This class tracks all relevant details
/// including order type, delivery location, package characteristics, and current delivery status.
/// </remarks>
public class Order
{
    /// <summary>
    /// Gets the unique identifier for the order.
    /// </summary>
    /// <value>A unique integer identifier assigned to the order at creation.</value>
    public int Id { get; init; }

    /// <summary>
    /// Gets or sets the type of order (Regular, Express, Heavy, Fragile, Refrigerated).
    /// </summary>
    /// <value>An OrderType enumeration value specifying the order category.</value>
    public OrderType orderType { get; set; }

    /// <summary>
    /// Gets or sets the description of the order.
    /// </summary>
    /// <value>A text description of the order contents or special instructions.</value>
    public string? DescriptionOfOrder { get; set; }

    /// <summary>
    /// Gets or sets the delivery address for the order.
    /// </summary>
    /// <value>The physical address where the order should be delivered.</value>
    public string AddressOfOrder { get; set; }

    /// <summary>
    /// Gets or sets the latitude coordinate of the delivery location.
    /// </summary>
    /// <value>The geographic latitude in decimal degrees.</value>
    public double Latitude { get; set; }

    /// <summary>
    /// Gets or sets the longitude coordinate of the delivery location.
    /// </summary>
    /// <value>The geographic longitude in decimal degrees.</value>
    public double Longitude { get; set; }

    /// <summary>
    /// Gets or sets the air distance from the company to the delivery location in kilometers.
    /// </summary>
    /// <value>The straight-line distance in kilometers.</value>
    public double AirDistance { get; set; }

    /// <summary>
    /// Gets or sets the name of the customer placing the order.
    /// </summary>
    /// <value>The customer's full name.</value>
    public string CustomerName { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the customer.
    /// </summary>
    /// <value>The customer's contact phone number.</value>
    public string CustomerPhone { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the order contains fragile items.
    /// </summary>
    /// <value>True if the order is fragile; otherwise, false.</value>
    public bool IsFrag { get; set; }

    /// <summary>
    /// Gets or sets the volume of the order in cubic units.
    /// </summary>
    /// <value>The volume measurement, or null if not specified.</value>
    public double? Volume { get; set; }

    /// <summary>
    /// Gets or sets the weight of the order in kilograms.
    /// </summary>
    /// <value>The weight measurement, or null if not specified.</value>
    public double? Weight { get; set; }

    /// <summary>
    /// Gets the date and time when the order was placed.
    /// </summary>
    /// <value>A DateTime representing when the order was created, or null if not set.</value>
    public DateTime? OrderPlacementTime { get; init; }

    /// <summary>
    /// Gets the expected completion time for the order delivery.
    /// </summary>
    /// <value>A DateTime representing the expected delivery time, or null if not calculated.</value>
    public DateTime? ExpectedCompletionTime { get; init; }

    /// <summary>
    /// Gets the maximum allowed delivery time for the order.
    /// </summary>
    /// <value>A DateTime representing the deadline for delivery.</value>
    public DateTime MaxDeliveryTime { get; init; }

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
    /// Gets the remaining time until delivery completion.
    /// </summary>
    /// <value>A DateTime representing the remaining time.</value>
    public DateTime RemainingTimeToDelivery { get; init; }

    /// <summary>
    /// Gets the delivery information for this order in a list view context.
    /// </summary>
    /// <value>A DeliveryPerOrderInList object, or null if no delivery is assigned.</value>
    public DeliveryPerOrderInList? DeliveryPerOrderInList { get; init; }

    /// <summary>
    /// Returns a string representation of the order object.
    /// </summary>
    /// <returns>A formatted string containing the order's properties.</returns>
    public override string ToString() => this.ToStringProperty();
}
