using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
namespace BO;



public class Order
{
    public int Id { get; init; }

    public OrderType OrderType { get; set; }
    public string AddressOfOrder { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string CustomerName { get; set; }
    public string CustomerPhone { get; set; }
    public bool IsFrag { get; set; }
    public double? Volume { get; set; }
    public double? Weight { get; set; }
    public string? DescriptionOfOrder { get; set; }
    public DateTime? OrderPlacementTime { get; set; }


    public override string ToString() => this.ToStringProperty();
}