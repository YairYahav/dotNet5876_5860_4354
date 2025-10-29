namespace DO;
/// <summary>
/// Order information.
/// </summary>
/// <param name="Id">The unique identifier for the order.</param>
/// <param name="OrderType">The type of the order.</param>
/// <param name="AddressOfOrder">The address where the order is to be delivered.</param>
/// <param name="Latitude">The latitude coordinate of the delivery address.</param> 
/// <param name="Longitude">The longitude coordinate of the delivery address.</param>
/// <param name="CustomerName">The name of the customer placing the order.</param>
/// <param name="CustomerPhone">The phone number of the customer placing the order.</param>
/// <param name="DescriptionOfOrder">Optional description of the order.</param>
/// <param name="IsFrag">Indicates whether the order is fragile.</param>
/// <param name="Volume">Optional volume of the order.</param>
/// <param name="Weight">Optional weight of the order.</param>
public record Order
(
    int Id,
    OrderType OrderType,
    string AddressOfOrder,
    double Latitude,
    double Longitude,
    string CustomerName,
    string CustomerPhone,
    bool IsFrag,
    double? Volume = null,
    double ? Weight = null,
    string? DescriptionOfOrder = null
)
{
    /// <summary>
    /// Default constructor 
    /// </summary>
    public Order() : this(0, default, "", 0.0, 0.0, "", "", false) { }
}
