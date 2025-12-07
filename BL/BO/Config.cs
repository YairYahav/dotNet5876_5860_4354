using System;
namespace BO;
using Helpers;

/// <summary>
/// Represents the configuration settings for the entire application.
/// Contains system-wide parameters, courier speeds, time ranges, and manager credentials.
/// </summary>
/// <remarks>
/// This class encapsulates all configurable application settings including clock management,
/// delivery parameters, courier speed specifications, and system constraints.
/// </remarks>
public class Config
{
    /// <summary>
    /// Gets or sets the current system clock time.
    /// </summary>
    /// <value>A DateTime representing the current application time.</value>
    public DateTime Clock { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the system manager.
    /// </summary>
    /// <value>An integer representing the manager's ID.</value>
    public int ManagerId { get; set; }

    /// <summary>
    /// Gets or sets the password for the manager's account.
    /// </summary>
    /// <value>The manager's authentication password.</value>
    public string ManagerPassword { get; set; }

    /// <summary>
    /// Gets or sets the physical address of the company.
    /// </summary>
    /// <value>The company's address as a string, or null if not specified.</value>
    public string? CompanyAddress { get; set; }

    /// <summary>
    /// Gets or sets the latitude coordinate of the company location.
    /// </summary>
    /// <value>The geographic latitude in decimal degrees, or null if not specified.</value>
    public double? Latitude { get; set; }

    /// <summary>
    /// Gets or sets the longitude coordinate of the company location.
    /// </summary>
    /// <value>The geographic longitude in decimal degrees, or null if not specified.</value>
    public double? Longitude { get; set; }

    /// <summary>
    /// Gets or sets the maximum delivery range for couriers in kilometers.
    /// </summary>
    /// <value>The maximum distance in kilometers, or null if unlimited.</value>
    public double? MaxDeliveryRange { get; set; }

    /// <summary>
    /// Gets or sets the average speed for car deliveries in kilometers per hour.
    /// </summary>
    /// <value>The average speed in km/h for car-based deliveries.</value>
    public double AvgSpeedKmhForCar { get; set; }

    /// <summary>
    /// Gets or sets the average speed for motorcycle deliveries in kilometers per hour.
    /// </summary>
    /// <value>The average speed in km/h for motorcycle-based deliveries.</value>
    public double AvgSpeedKmhForMotorcycle { get; set; }

    /// <summary>
    /// Gets or sets the average speed for bicycle deliveries in kilometers per hour.
    /// </summary>
    /// <value>The average speed in km/h for bicycle-based deliveries.</value>
    public double AvgSpeedKmhForBicycle { get; set; }

    /// <summary>
    /// Gets or sets the average speed for foot deliveries in kilometers per hour.
    /// </summary>
    /// <value>The average speed in km/h for on-foot deliveries.</value>
    public double AvgSpeedKmhForFoot { get; set; }

    /// <summary>
    /// Gets or sets the maximum time range allowed for delivery completion.
    /// </summary>
    /// <value>A TimeSpan representing the maximum delivery duration.</value>
    public TimeSpan MaxTimeRangeForDelivery { get; set; }

    /// <summary>
    /// Gets or sets the risk range for deliveries approaching their deadline.
    /// </summary>
    /// <value>A TimeSpan representing the risk period before the deadline.</value>
    public TimeSpan RiskRange { get; set; }

    /// <summary>
    /// Gets or sets the inactivity range for couriers.
    /// </summary>
    /// <value>A TimeSpan representing the maximum allowed inactivity period.</value>
    public TimeSpan InactivityRange { get; set; }

    /// <summary>
    /// Returns a string representation of the configuration object.
    /// </summary>
    /// <returns>A formatted string containing the configuration properties.</returns>
    public override string ToString() => this.ToStringProperty();
}