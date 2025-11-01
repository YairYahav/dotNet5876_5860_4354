namespace DalApi;
/// <summary>
/// Interface for managing configuration settings in the Data Access Layer (DAL).
/// </summary>
public interface IConfig
{
    DateTime Clock { get; set; }


    int ManagerId { get; set; } // Unique identifier for the manager
    string ManagerPassword { get; set; } // Password for the manager's account
    string? CompanyAddress { get; set; } // Company's physical address
    double? Latitude { get; set; } // Geographic latitude of the location of order
    double? Longitude { get; set; }// Geographic longitude of the location of order
    double? MaxDeliveryRange { get; set; }// Maximum delivery range for couriers
    double AvgSpeedKmhForCar { get; set; }// Average speed for car deliveries in km/h
    double AvgSpeedKmhForMotorcycle { get; set; }// Average speed for motorcycle deliveries in km/h
    double AvgSpeedKmhForBicycle { get; set; }// Average speed for bicycle deliveries in km/h
    double AvgSpeedKmhForFoot { get; set; }// Average speed for foot deliveries in km/h
    TimeSpan MaxTimeRangeForDelivery { get; set; }// Maximum time range allowed for delivery
    TimeSpan RiskRange { get; set; }// Risk range for deliveries
    TimeSpan InactivityRange { get; set; }// Inactivity range for couriers


    void Reset();// Method to reset configuration settings to default values
}
