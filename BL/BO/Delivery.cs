using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;

namespace BO;


public class Delivery
{

    public int Id { get; init; }


    public int OrderId { get; set; }
    public int CourierId { get; set; }


    public DeliveryType DeliveryType { get; set; }
    public DateTime DeliveryStartTime { get; set; }
    public double? ActualDistance { get; set; }
    public DateTime? DeliveryCompletionTime { get; set; }
    public TypeOfDeliveryCompletionTime? TypeOfDeliveryCompletionTime { get; set; }


    public override string ToString() => this.ToStringProperty();
}