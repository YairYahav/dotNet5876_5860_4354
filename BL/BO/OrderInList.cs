namespace BO;
using System;
using Helpers;


public class OrderInList
{
    public int Id { get; init; }
    public OrderType OrderType { get; set; }
    public string CustomerName { get; set; }
    public DateTime? OrderPlacementTime { get; set; }


    public override string ToString() => this.ToStringProperty();
}