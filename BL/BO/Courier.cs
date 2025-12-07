namespace BO;
using System;
using DO;
using Helpers;

/// <summary>
/// Represents a courier entity in the business logic layer.
/// Contains personal information, delivery preferences, and performance metrics.
/// </summary>
/// <remarks>
/// A courier is a delivery professional who handles orders using a specified delivery type.
/// This class tracks their activity, employment dates, and delivery performance.
/// </remarks>
public class Courier
{
    /// <summary>
    /// Gets or sets the unique identifier for the courier.
    /// </summary>
    /// <value>A unique integer identifier assigned to each courier.</value>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the full name of the courier.
    /// </summary>
    /// <value>The courier's complete name as a string.</value>
    public string FullName { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the courier.
    /// </summary>
    /// <value>The courier's contact phone number.</value>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the email address of the courier.
    /// </summary>
    /// <value>The courier's Gmail address.</value>
    public string Gmail { get; set; }

    /// <summary>
    /// Gets or sets the password for the courier's account.
    /// </summary>
    /// <value>The courier's authentication password.</value>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the courier is currently active.
    /// </summary>
    /// <value>True if the courier is active; otherwise, false.</value>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the type of delivery this courier specializes in.
    /// </summary>
    /// <value>A DeliveryType enumeration value (Car, Motorcycle, Bicycle, OnFoot).</value>
    public DeliveryType DeliveryType { get; set; }

    /// <summary>
    /// Gets or sets the maximum personal delivery distance the courier can travel in kilometers.
    /// </summary>
    /// <value>The maximum delivery distance in kilometers, or null if unlimited.</value>
    public double? MaxPersonalDeliveryDistance { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the courier started employment.
    /// </summary>
    /// <value>A DateTime representing the employment start date.</value>
    public DateTime EmploymentStartDate { get; set; }

    /// <summary>
    /// Gets the number of deliveries completed on time by this courier.
    /// </summary>
    /// <value>The count of on-time deliveries.</value>
    public int NumberOfDeliveriesCompletedOnTime { get; init; }

    /// <summary>
    /// Gets the number of deliveries completed late by this courier.
    /// </summary>
    /// <value>The count of late deliveries.</value>
    public int NumberOfDeliveriesCompletedLate { get; init; }

    /// <summary>
    /// Gets or sets the current order in progress for this courier.
    /// </summary>
    /// <value>An OrderInProgress object if the courier has an active delivery; otherwise, null.</value>
    public OrderInProgress? ordersInProgress { get; set; }

    /// <summary>
    /// Returns a string representation of the courier object.
    /// </summary>
    /// <returns>A formatted string containing the courier's properties.</returns>
    public override string ToString() => this.ToStringProperty();
}