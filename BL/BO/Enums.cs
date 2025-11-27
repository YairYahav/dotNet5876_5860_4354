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

public enum OrderStatus
{
    Open,
    InProgress,
    Delivered,
    DeliveryDeclinedByCustomer,
    Canceled
}

public enum ScheduleStatus
{
    OnTime,
    InRisk,
    Late
}

public enum CourierListOrderBy
{
    
}

public enum OrderListFilterBy
{
    
}

public enum OrderListOrderBy
{
    
}

public enum ClosedDeliveryListFilterBy
{

}

public enum ClosedDeliveryListOrderBy
{

}

public enum OpenDeliveryListFilterBy
{
}

public enum OpenDeliveryListOrderBy
{

}
public enum UserRole
{
    Admin,
    Courier
}
