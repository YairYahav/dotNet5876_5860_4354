namespace DO;
/// <summary>
/// Courier record representing a delivery person.
/// </summary>
/// <param name="Id">The unique identifier for the delivery person</param>
/// <param name="FullName">The Full name of the delivery guy</param>
/// <param name="PhoneNumber">Phone number of the delivery guy</param>
/// <param name="Gmail">Gmail of the delivery guy</param>
/// <param name="Password">Password for the delivery guy's account</param>
/// <param name="IsActive">Indicates whether the delivery guy is currently active</param>
/// <param name="DeliveryType">The type of deliveries the courier handles</param>
/// <param name="EmploymentStartDate">The date when the courier started employment</param>
/// <param name="MaxPersonalDeliveryDistance">The maximum distance the courier is willing to deliver (optional)</param>

public record Courier
(
    int Id,
    string FullName,
    string PhoneNumber,
    string Gmail,
    string Password,
    bool IsActive,
    DeliveryType DeliveryType,
    DateTime EmploymentStartDate,
    double? MaxPersonalDeliveryDistance = null
)
{
    /// <summary>
    /// Default constructor 
    /// </summary>
    public Courier() : this(0, "", "", "", "", false, default, default) { }
}
