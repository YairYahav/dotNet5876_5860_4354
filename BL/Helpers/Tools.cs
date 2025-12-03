namespace Helpers;
using DalApi;
using System.Net.Http;
using System.Text.Json;
using System.Globalization;
using System.Collections;
using System.Reflection;
using System.Text;



internal static class Tools
{
    private static readonly IDal s_dal = Factory.Get;
    private const string LocationIqBaseUrl = "https://us1.locationiq.com/v1";
    
    // קבל את ה-API Key ממשתנות סביבה
    private static string LocationIqApiKey => 
        Environment.GetEnvironmentVariable("LOCATIONIQ_API_KEY") ?? 
        throw new InvalidOperationException("LOCATIONIQ_API_KEY environment variable is not set. Please set it before running the application.");
    
    private static readonly HttpClient s_httpClient = new();

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
                    // For collections (except strings), list the count or items
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


    internal static void AuthorizeAdmin(int requesterId)
    {
        var u = AdminManager.GetConfig().ManagerId;
        if (requesterId != u)
            throw new BO.BlUnauthorizedAccessException("Only admin users are authorized to perform this action.");
    }
    
    internal static bool IsAdmin(int requesterId)
    {
        return requesterId == s_dal.Config.ManagerId;
    }


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
    
    private const double EarthRadiusKm = 6371.0;

    private static double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }

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

        // קריאה סינכרונית לפי דרישת שלב 4
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
    
    internal static double CalculateActualDistance(double orderLatitude, double orderLongitude, DO.DeliveryType deliveryType)
    {
        double companyLatitude = s_dal.Config.Latitude.GetValueOrDefault();
        double companyLongitude = s_dal.Config.Longitude.GetValueOrDefault();

        // פרופיל המסלול
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
            // אם יש תקלה ברשת או ב API נ fallback למרחק אווירי
            return AirDistance(companyLatitude, companyLongitude, orderLatitude, orderLongitude);
        }
    }

    private static double GetRouteDistanceKm(double fromLat, double fromLon, double toLat, double toLon, string profile)
    {
        if (string.IsNullOrWhiteSpace(LocationIqApiKey))
            throw new BO.BlDataValidationException("LocationIQ API key is missing - set LOCATIONIQ_API_KEY environment variable");

        // שים לב: Directions של LocationIQ מקבל lon,lat בסדר הזה
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
