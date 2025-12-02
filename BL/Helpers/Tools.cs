namespace Helpers;
using DalApi;
using System.Reflection;
using System.Collections;
using System.Text;


internal static class Tools
{
    private static readonly IDal s_dal = Factory.Get;

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
        var u = s_dal.Config.ManagerId;
        if (requesterId != s_dal.Config.ManagerId)
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

    /// <summary>
    /// Calculates the actual distance for a delivery based on the delivery type and order coordinates.
    /// 
    /// Distance calculation rules according to specifications:
    /// - Car and Motorcycle use the same route (driving route)
    /// - Bicycle and OnFoot use the same route (walking/cycling route)
    /// 
    /// In Stage 4, this method will integrate with a geocoding service to calculate
    /// the actual route distance. For now, it uses air distance as the basis.
    /// </summary>
    /// <param name="orderLatitude">Latitude of the order delivery address</param>
    /// <param name="orderLongitude">Longitude of the order delivery address</param>
    /// <param name="deliveryType">Type of delivery (Car, Motorcycle, Bicycle, OnFoot)</param>
    /// <returns>The actual distance in kilometers based on delivery type</returns>
    internal static double CalculateActualDistance(double orderLatitude, double orderLongitude, DO.DeliveryType deliveryType)
    {
        // Get company coordinates
        double companyLatitude = s_dal.Config.Latitude.GetValueOrDefault();
        double companyLongitude = s_dal.Config.Longitude.GetValueOrDefault();

        // For now, we use air distance as a basis
        // In Stage 4, this will call a geocoding service to get actual route distance
        double distance = AirDistance(companyLatitude, companyLongitude, orderLatitude, orderLongitude);

        // In Stage 4: Different delivery types will have different route calculations
        // Car and Motorcycle: same driving route
        // Bicycle and OnFoot: same walking/cycling route
        // For now, return the calculated distance
        return distance;
    }
}
