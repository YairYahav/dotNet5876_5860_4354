using DalApi;

namespace Dal;
/// <summary>
/// Implementation of the IConfig interface for managing configuration settings in the Data Access Layer (DAL).
/// </summary>

public class ConfigImplementation : IConfig
{
    public DateTime Clock { get => Config.Clock; set => Config.Clock = value; }// Current application clock time.
    public int ManagerId { get => Config.ManagerId; set => Config.ManagerId = value; }// Unique identifier for the manager
    public string ManagerPassword { get => Config.ManagerPassword; set => Config.ManagerPassword = value; }// Password for the manager's account
    public string? CompanyAddress { get => Config.CompanyAddress; set => Config.CompanyAddress = value; }// Company's physical address
    public double? Latitude { get => Config.Latitude; set => Config.Latitude = value; }// Geographic latitude of the location of order
    public double? Longitude { get => Config.Longitude; set => Config.Longitude = value; }// Geographic longitude of the location of order
    public double? MaxDeliveryRange { get => Config.MaxDeliveryRange; set => Config.MaxDeliveryRange = value; }// Maximum delivery range for couriers
    public double AvgSpeedKmhForCar { get => Config.AvgSpeedKmhForCar; set => Config.AvgSpeedKmhForCar = value; }// Average speed for car deliveries in km/h
    public double AvgSpeedKmhForMotorcycle { get => Config.AvgSpeedKmhForMotorcycle; set => Config.AvgSpeedKmhForMotorcycle = value; }// Average speed for motorcycle deliveries in km/h
    public double AvgSpeedKmhForBicycle { get => Config.AvgSpeedKmhForBicycle; set => Config.AvgSpeedKmhForBicycle = value; }// Average speed for bicycle deliveries in km/h
    public double AvgSpeedKmhForFoot { get => Config.AvgSpeedKmhForFoot; set => Config.AvgSpeedKmhForFoot = value; }// Average speed for foot deliveries
    public TimeSpan MaxTimeRangeForDelivery { get => Config.MaxTimeRangeForDelivery; set => Config.MaxTimeRangeForDelivery = value; }// Maximum time range allowed for delivery
    public TimeSpan RiskRange { get => Config.RiskRange; set => Config.RiskRange = value; }// Risk range for deliveries
    public TimeSpan InactivityRange { get => Config.InactivityRange; set => Config.InactivityRange = value; }// Inactivity range for couriers

    public void Reset()// Method to reset configuration settings to default values
    {
        Config.Reset();
    }
}
