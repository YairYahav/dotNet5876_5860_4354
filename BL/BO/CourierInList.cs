namespace BO;
using Helpers;


public class CourierInList
{
    public int Id { get; init; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public DeliveryType DeliveryType { get; set; }


    public override string ToString() => this.ToStringProperty();
}