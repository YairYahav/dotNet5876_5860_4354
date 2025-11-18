using System;
namespace BO;
using Helpers;


public class Config
{
    public DateTime Clock { get; set; }

    public int ManagerId { get; set; }
    public string ManagerPassword { get; set; }

    public string? CompanyAddress { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? MaxDeliveryRange { get; set; }

    public double AvgSpeedKmhForCar { get; set; }
    public double AvgSpeedKmhForMotorcycle { get; set; }
    public double AvgSpeedKmhForBicycle { get; set; }
    public double AvgSpeedKmhForFoot { get; set; }
    public TimeSpan MaxTimeRangeForDelivery { get; set; }
    public TimeSpan RiskRange { get; set; }
    public TimeSpan InactivityRange { get; set; }

    public override string ToString() => this.ToStringProperty();

}