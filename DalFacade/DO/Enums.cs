namespace DO;
/// <summary>
/// Types of order
/// Regular, Express, Heavy, Fragile, Refrigerated
/// </summary>
public enum OrderType
{
    Regular,
    Express,
    Heavy,
    Fragile,
    Refrigerated
}


/// <summary>
/// Types of delivery completion time
/// Supplied, Failed, Canceled, RefusedByCustomer, CustomerNotFound
/// </summary>
public enum TypeOfDeliveryCompletionTime
{
    Supplied,
    Failed,
    Canceled,
    RefusedByCustomer,
    CustomerNotFound
}

/// <summary>
/// Types of delivery
/// Car, Motorcycle, Bicycle, OnFoot
/// </summary>
public enum DeliveryType
{
    Car,
    Motorcycle,
    Bicycle,
    OnFoot
}