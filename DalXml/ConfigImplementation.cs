using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal;
using DalApi;
using DO;



internal class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }


    public int ManagerId
    {
        get => Config.ManagerId;
        set => Config.ManagerId = value;
    }


    public string ManagerPassword
    {
        get => Config.ManagerPassword;
        set => Config.ManagerPassword = value;
    }


    public string? CompanyAddress
    {
        get => Config.CompanyAddress;
        set => Config.CompanyAddress = value;
    }


    public double? Latitude
    {
        get => Config.Latitude;
        set => Config.Latitude = value;
    }


    public double? Longitude
    {
        get => Config.Longitude;
        set => Config.Longitude = value;
    }


    public double? MaxDeliveryRange
    {
        get => Config.MaxDeliveryRange;
        set => Config.MaxDeliveryRange = value;
    }


    public double AvgSpeedKmhForCar
    {
        get => Config.AvgSpeedKmhForCar;
        set => Config.AvgSpeedKmhForCar = value;
    }


    public double AvgSpeedKmhForMotorcycle
    {
        get => Config.AvgSpeedKmhForMotorcycle;
        set => Config.AvgSpeedKmhForMotorcycle = value;
    }


    public double AvgSpeedKmhForBicycle
    {
        get => Config.AvgSpeedKmhForBicycle;
        set => Config.AvgSpeedKmhForBicycle = value;
    }


    public double AvgSpeedKmhForFoot
    {
        get => Config.AvgSpeedKmhForFoot;
        set => Config.AvgSpeedKmhForFoot = value;
    }


    public TimeSpan MaxTimeRangeForDelivery
    {
        get => Config.MaxTimeRangeForDelivery;
        set => Config.MaxTimeRangeForDelivery = value;
    }


    public TimeSpan RiskRange
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }


    public TimeSpan InactivityRange
    {
        get => Config.InactivityRange;
        set => Config.InactivityRange = value;
    }


    public void Reset()
    {
        Config.Reset();
    }
}
