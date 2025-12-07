namespace Helpers;
using DalApi;
using System.Net.Http;
using System.Text.Json;
using System.Globalization;
using System.Collections;
using System.Reflection;
using System.Text;

/// <summary>
/// Provides utility functions for authorization, distance calculations, geocoding, and object string representation.
/// </summary>
/// <remarks>
/// This static class contains helper methods used throughout the business logic layer, including
/// administrative authorization checks, geographic distance calculations using the Haversine formula,
/// and integration with the LocationIQ API for address geocoding and route distance calculation.
/// </remarks>
internal static class Tools
{
    /// <summary>
    /// Gets the Data Access Layer (DAL) factory instance for accessing configuration.
    /// </summary>
    private static readonly IDal s_dal = Factory.Get;

    /// <summary>
    /// Gets the base URL for the LocationIQ API service.
    /// </summary>
    private const string LocationIqBaseUrl = "https://us1.locationiq.com/v1";
    
    /// <summary>
    /// Gets the LocationIQ API key from environment variables.
    /// </summary>
    /// <remarks>The API key is retrieved from the LOCATIONIQ_API_KEY environment variable.
    /// An InvalidOperationException is thrown if the variable is not set.</remarks>
    private static string LocationIqApiKey => 
        Environment.GetEnvironmentVariable("LOCATIONIQ_API_KEY") ?? 
        throw new InvalidOperationException("LOCATIONIQ_API_KEY environment variable is not set. Please set it before running the application.");
    
    /// <summary>
    /// Gets a shared HTTP client instance for making web requests.
    /// </summary>
    private static readonly HttpClient s_httpClient = new();

    /// <summary>
    /// Converts an object to a formatted string representation of its properties.
    /// </summary>
    /// <typeparam name="T">The type of the object to convert.</typeparam>
    /// <param name="t">The object to convert to a string.</param>
    /// <returns>A formatted string showing all public properties and their values.</returns>
    /// <remarks>
    /// This method uses reflection to enumerate all public properties of the object.
    /// Collections are shown as counts, and errors in retrieving values are handled gracefully.
    /// </remarks>
    public static string ToStringProperty<T>(this T t)
    {
        if (t == null) return "null";

        var sb = new StringBuilder();
        Type type = t.GetType();

        sb.AppendLine($"--- {type.Name} ---");

        foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            try
            {
                object? value = prop.GetValue(t);
                string valueString;

                if (value is null)
                {
                    valueString = "null";
                }
                else if (value is IEnumerable enumerable and not string)
                {
                    int count = enumerable.Cast<object>().Count();
                    valueString = count > 0 ? $"Collection with {count} items" : "Empty Collection";
                }
                else
                {
                    valueString = value.ToString() ?? "null";
                }

                sb.AppendLine($"  {prop.Name}: {valueString}");
            }
            catch (Exception ex)
            {
                sb.AppendLine($"  {prop.Name}: [Error retrieving value: {ex.Message}]");
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// Authorizes a user request to require admin access.
    /// </summary>
    /// <param name="requesterId">The ID of the user making the request.</param>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the requester is not the system manager (admin).</exception>
    internal static void AuthorizeAdmin(int requesterId)
    {
        var u = AdminManager.GetConfig().ManagerId;
        if (requesterId != u)
            throw new BO.BlUnauthorizedAccessException("Only admin users are authorized to perform this action.");
    }
    
    /// <summary>
    /// Checks if a user has admin privileges.
    /// </summary>
    /// <param name="requesterId">The ID of the user to check.</param>
    /// <returns>True if the user is the system manager; otherwise, false.</returns>
    internal static bool IsAdmin(int requesterId)
    {
        return requesterId == AdminManager.GetConfig().ManagerId;
    }

    /// <summary>
    /// Calculates the straight-line air distance between two geographic coordinates using the Haversine formula.
    /// </summary>
    /// <param name="lat1">The latitude of the first location in decimal degrees.</param>
    /// <param name="lon1">The longitude of the first location in decimal degrees.</param>
    /// <param name="lat2">The latitude of the second location in decimal degrees.</param>
    /// <param name="lon2">The longitude of the second location in decimal degrees.</param>
    /// <returns>The air distance in kilometers.</returns>
    /// <remarks>This method uses the Haversine formula to calculate the great-circle distance between two points on Earth, using the Earth's mean radius of 6371 km.</remarks>
    internal static double AirDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double dLat = DegreesToRadians(lat2 - lat1);
        double dLon = DegreesToRadians(lon2 - lon1);

        lat1 = DegreesToRadians(lat1);
        lat2 = DegreesToRadians(lat2);


        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusKm * c;
    }
    
    /// <summary>
    /// The Earth's mean radius in kilometers used for distance calculations.
    /// </summary>
    private const double EarthRadiusKm = 6371.0;

    /// <summary>
    /// Converts an angle in degrees to radians.
    /// </summary>
    /// <param name="degrees">The angle in degrees.</param>
    /// <returns>The angle in radians.</returns>
    private static double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }

    /// <summary>
    /// Converts an address string to latitude and longitude coordinates using the LocationIQ geocoding service.
    /// </summary>
    /// <param name="address">The street address to geocode.</param>
    /// <returns>A tuple containing (Latitude, Longitude) of the address.</returns>
    /// <exception cref="BO.BlDataValidationException">Thrown if the address is empty, API key is missing, or address is not found.</exception>
    /// <remarks>This method uses the LocationIQ Search API to convert an address string into geographic coordinates. The LOCATIONIQ_API_KEY environment variable must be set.</remarks>
    internal static (double Latitude, double Longitude) GetCoordinatesForAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new BO.BlDataValidationException("Address cannot be empty for geocoding");

        if (string.IsNullOrWhiteSpace(LocationIqApiKey))
            throw new BO.BlDataValidationException("LocationIQ API key is missing - set LOCATIONIQ_API_KEY environment variable");

        string url = $"{LocationIqBaseUrl}/search" +
                     $"?key={LocationIqApiKey}" +
                     $"&q={Uri.EscapeDataString(address)}" +
                     $"&format=json";

        var response = s_httpClient.GetAsync(url).Result;
        response.EnsureSuccessStatusCode();

        string json = response.Content.ReadAsStringAsync().Result;

        using JsonDocument doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
            throw new BO.BlDataValidationException("Address not found by geocoding service");

        var first = root[0];

        string? latStr = first.GetProperty("lat").GetString();
        string? lonStr = first.GetProperty("lon").GetString();

        if (!double.TryParse(latStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double lat))
            throw new BO.BlDataValidationException("Invalid latitude from geocoding service");

        if (!double.TryParse(lonStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double lon))
            throw new BO.BlDataValidationException("Invalid longitude from geocoding service");

        return (lat, lon);
    }
    
    /// <summary>
    /// Calculates the actual travel distance from the company location to an order location, considering the delivery method's preferred route type.
    /// </summary>
    /// <param name="orderLatitude">The latitude of the order's delivery location.</param>
    /// <param name="orderLongitude">The longitude of the order's delivery location.</param>
    /// <param name="deliveryType">The type of delivery (Car, Motorcycle, Bicycle, OnFoot).</param>
    /// <returns>The travel distance in kilometers based on the optimal route for the delivery type.</returns>
    /// <remarks>This method attempts to use the LocationIQ routing service to get actual route distances. If the API fails, it falls back to calculating air distance.</remarks>
    internal static double CalculateActualDistance(double orderLatitude, double orderLongitude, DO.DeliveryType deliveryType)
    {
        double companyLatitude = AdminManager.GetConfig().Latitude.GetValueOrDefault();
        double companyLongitude = AdminManager.GetConfig().Longitude.GetValueOrDefault();

        string profile = deliveryType switch
        {
            DO.DeliveryType.Car => "driving",
            DO.DeliveryType.Motorcycle => "driving",
            DO.DeliveryType.Bicycle => "cycling",
            DO.DeliveryType.OnFoot => "foot",
            _ => "driving"
        };

        try
        {
            double distanceKm = GetRouteDistanceKm(companyLatitude, companyLongitude,
                                                   orderLatitude, orderLongitude,
                                                   profile);
            return distanceKm;
        }
        catch
        {
            return AirDistance(companyLatitude, companyLongitude, orderLatitude, orderLongitude);
        }
    }

    /// <summary>
    /// Calculates the actual route distance between two points using the LocationIQ routing service.
    /// </summary>
    /// <param name="fromLat">The latitude of the starting location.</param>
    /// <param name="fromLon">The longitude of the starting location.</param>
    /// <param name="toLat">The latitude of the destination location.</param>
    /// <param name="toLon">The longitude of the destination location.</param>
    /// <param name="profile">The routing profile (driving, cycling, foot, etc.).</param>
    /// <returns>The route distance in kilometers.</returns>
    /// <exception cref="BO.BlDataValidationException">Thrown if API key is missing or no route is found.</exception>
    /// <remarks>This method uses the LocationIQ Directions API to calculate actual travel distances based on road networks and the specified travel mode.</remarks>
    private static double GetRouteDistanceKm(double fromLat, double fromLon, double toLat, double toLon, string profile)
    {
        if (string.IsNullOrWhiteSpace(LocationIqApiKey))
            throw new BO.BlDataValidationException("LocationIQ API key is missing - set LOCATIONIQ_API_KEY environment variable");

        string coordinates = $"{fromLon.ToString(CultureInfo.InvariantCulture)},{fromLat.ToString(CultureInfo.InvariantCulture)};" +
                             $"{toLon.ToString(CultureInfo.InvariantCulture)},{toLat.ToString(CultureInfo.InvariantCulture)}";

        string url = $"{LocationIqBaseUrl}/directions/{profile}/{coordinates}" +
                     $"?key={LocationIqApiKey}&overview=false";

        var response = s_httpClient.GetAsync(url).Result;
        response.EnsureSuccessStatusCode();

        string json = response.Content.ReadAsStringAsync().Result;
        using JsonDocument doc = JsonDocument.Parse(json);

        var root = doc.RootElement;

        if (!root.TryGetProperty("routes", out JsonElement routes) ||
            routes.ValueKind != JsonValueKind.Array ||
            routes.GetArrayLength() == 0)
        {
            throw new BO.BlDataValidationException("No route found by routing service");
        }

        double distanceMeters = routes[0].GetProperty("distance").GetDouble();
        return distanceMeters / 1000.0;
    }
}
