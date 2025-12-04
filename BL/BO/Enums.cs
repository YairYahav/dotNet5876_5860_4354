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
    None,
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
    ById,
    ByName,
    ByActive,
    EmploymentStartDate,
    DeliveryType
}

public enum OrderListFilterBy
{
    ByStatus,
    ByTiming,
    
}

public enum OrderListOrderBy
{
    ById,
    ByStatus,
    ByTiming,
    ByDistance

}

public enum ClosedDeliveryListFilterBy
{
    ByTypeOfCompletion,
    ByDeliveryType,
    ByTiming,
}

public enum ClosedOrdersListOrderBy
{
    ByTypeOfCompletion,
    ByOrderType,
    ByTiming,
}

public enum OpenDeliveryListFilterBy
{
    ByOrderType,
    ByTiming,
}

public enum OpenDeliveryListOrderBy
{
    ById,
    ByDistance,
    ByTiming,
}
public enum UserRole
{
    Admin,
    Courier
}
