namespace Dal;
/// <summary>
/// Configuration class for managing application settings and unique identifiers.
/// </summary>
internal static class Config
{
    /// <summary>
    /// Starting ID for orders.
    /// Next unique order ID.
    /// </summary>
    internal const int startOrderId = 1000;
    private static int nextOrderId = startOrderId;
    internal static int NextOrderId { get => nextOrderId++; }

    /// <summary>
    /// Starting ID for deliveries.
    /// Next unique delivery ID.
    /// </summary>
    internal const int startDeliveryId = 1000;
    private static int nextDeliveryId = startDeliveryId;
    internal static int NextDeliveryId { get => nextDeliveryId++; }

    /// <summary> Current application clock time. </summary>
    internal static DateTime Clock { get; set; }
    /// <summary> Manager's unique identifier. </summary>
    internal static int ManagerId { get; set; }
    /// <summary> Manager's password. </summary>
    internal static string ManagerPassword { get; set; } = "Admin1234";
    /// <summary> Company address. </summary>
    internal static string? CompanyAddress { get; set; } = null;
    /// <summary> Represents how far the point is north or south of the equator. </summary>
    internal static double? Latitude { get; set; } = null;
    /// <summary> Represents how far the point is east or west of the prime meridian. </summary> 
    internal static double? Longitude { get; set; } = null;
    /// <summary> Maximum delivery range for couriers for calculating distances. </summary>
    internal static double? MaxDeliveryRange { get; set; } = null;
    /// <summary> Average speeds for different delivery methods in km/h. </summary>
    internal static double AvgSpeedKmhForCar { get; set; }
    /// <summary> Average speed for motorcycle deliveries in km/h. </summary>
    internal static double AvgSpeedKmhForMotorcycle { get; set; }
    /// <summary> Average speed for bicycle deliveries in km/h. </summary>
    internal static double AvgSpeedKmhForBicycle { get; set; }
    /// <summary> Average speed for foot deliveries. </summary>
    internal static double AvgSpeedKmhForFoot { get; set; }
    /// <summary> Maximum time range allowed for delivery. </summary>
    internal static TimeSpan MaxTimeRangeForDelivery { get; set; } = TimeSpan.Zero;
    /// <summary> Risk range for deliveries. </summary>
    internal static TimeSpan RiskRange { get; set; } = TimeSpan.Zero;
    /// <summary> Inactivity range for couriers. </summary>
    internal static TimeSpan InactivityRange { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// constructor to reset all configuration settings to their default values.
    /// </summary>
    internal static void Reset()
    {
        nextOrderId = startOrderId;
        nextDeliveryId = startDeliveryId;
        Clock = DateTime.Now;
        ManagerId = 0;
        ManagerPassword = "Admin1234";
        AvgSpeedKmhForCar = 0;
        AvgSpeedKmhForMotorcycle = 0;
        AvgSpeedKmhForBicycle = 0;
        AvgSpeedKmhForFoot = 0;
        MaxTimeRangeForDelivery = TimeSpan.Zero;
        RiskRange = TimeSpan.Zero;
        InactivityRange = TimeSpan.Zero;
    }
}
