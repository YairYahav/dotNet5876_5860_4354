namespace BO;
using Helpers;


public class CourierInList
{
    public int Id { get; init; }
    public string FullName { get; set; }
    public bool IsActive { get; set; }
    public DeliveryType DeliveryType { get; set; }
    public DateTime EmploymentStartDate { get; set; }
    public int NumberOfDeliveriesCourierCompletedOnTime { get; set; }
    public int NumberOfDeliveriesCourierCompletedLate { get; set; }
    public OrderInProgress? CurrentOrderInProgress { get; set; }


    public override string ToString() => this.ToStringProperty();
}