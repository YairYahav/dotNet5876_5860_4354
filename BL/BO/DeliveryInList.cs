namespace BO;
using System;
using Helpers;


public class DeliveryInList
{
    public int Id { get; init; }
    public int OrderId { get; set; }
    public string CustomerName { get; set; }
    public int CourierId { get; set; }
    public DeliveryType DeliveryType { get; set; }
    public DateTime DeliveryStartTime { get; set; }
    public bool IsCompleted { get; set; }


    public override string ToString() => this.ToStringProperty();
}