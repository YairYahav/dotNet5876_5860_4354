namespace BO;
using System;
using DO;
using Helpers;


public class Courier
{
    public int Id { get; init; }
    public DateTime EmploymentStartDate { get; init; }


    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Gmail { get; set; }
    public string Password { get; set; }
    public bool IsActive { get; set; }
    public DeliveryType DeliveryType { get; set; }
    public double? MaxPersonalDeliveryDistance { get; set; }

    public override string ToString() => this.ToStringProperty();
}