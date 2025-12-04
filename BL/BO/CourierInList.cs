namespace BO;
using Helpers;


public class CourierInList
{
    public int Id { get; init; }
    public string FullName { get; init; }
    public string PhoneNumber { get; init; }
    public bool IsActive { get; init; }
    public DeliveryType DeliveryType { get; init; }
    public DateTime EmploymentStartDate { get; init; }
    public int NumberOfDeliveriesCourierCompletedOnTime { get; init; }
    public int NumberOfDeliveriesCourierCompletedLate { get; init; }
    public OrderInProgress? CurrentOrderInProgress { get; init; }



    public override string ToString() => this.ToStringProperty();
}