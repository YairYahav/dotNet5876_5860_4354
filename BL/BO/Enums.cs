namespace BO;


public enum TimeUnit
{
    MINUTE,
    HOUR,
    DAY,
    YEAR
}


public enum OrderType
{
    Regular,
    Express,
    Heavy,
    Fragile,
    Refrigerated
}


public enum TypeOfDeliveryCompletionTime
{
    Supplied,
    Failed,
    Canceled,
    RefusedByCustomer,
    CustomerNotFound
}


public enum DeliveryType
{
    Car,
    Motorcycle,
    Bicycle,
    OnFoot
}
