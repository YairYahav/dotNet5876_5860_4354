namespace BO;
using Helpers;

/// <summary>
/// Represents a courier's information in a list view format.
/// Contains essential courier details and delivery performance metrics.
/// </summary>
/// <remarks>
/// This class is used to display courier information in lists or collections,
/// providing a view-optimized representation of a courier with performance statistics
/// and current delivery status.
/// </remarks>
public class CourierInList
{
    /// <summary>
    /// Gets the unique identifier for the courier.
    /// </summary>
    /// <value>A unique integer identifier assigned to the courier.</value>
    public int Id { get; init; }

    /// <summary>
    /// Gets the full name of the courier.
    /// </summary>
    /// <value>The courier's complete name.</value>
    public string FullName { get; init; }

    /// <summary>
    /// Gets the phone number of the courier.
    /// </summary>
    /// <value>The courier's contact phone number.</value>
    public string PhoneNumber { get; init; }

    /// <summary>
    /// Gets a value indicating whether the courier is currently active.
    /// </summary>
    /// <value>True if the courier is active; otherwise, false.</value>
    public bool IsActive { get; init; }

    /// <summary>
    /// Gets the type of delivery this courier specializes in.
    /// </summary>
    /// <value>A DeliveryType enumeration value (Car, Motorcycle, Bicycle, OnFoot).</value>
    public DeliveryType DeliveryType { get; init; }

    /// <summary>
    /// Gets the date and time when the courier started employment.
    /// </summary>
    /// <value>A DateTime representing the employment start date.</value>
    public DateTime EmploymentStartDate { get; init; }

    /// <summary>
    /// Gets the number of deliveries completed on time by this courier.
    /// </summary>
    /// <value>The count of on-time deliveries.</value>
    public int NumberOfDeliveriesCourierCompletedOnTime { get; init; }

    /// <summary>
    /// Gets the number of deliveries completed late by this courier.
    /// </summary>
    /// <value>The count of late deliveries.</value>
    public int NumberOfDeliveriesCourierCompletedLate { get; init; }

    /// <summary>
    /// Gets the current order in progress for this courier.
    /// </summary>
    /// <value>An OrderInProgress object if the courier has an active delivery; otherwise, null.</value>
    public OrderInProgress? CurrentOrderInProgress { get; init; }

    /// <summary>
    /// Returns a string representation of the courier list item.
    /// </summary>
    /// <returns>A formatted string containing the courier's properties.</returns>
    public override string ToString() => this.ToStringProperty();
}