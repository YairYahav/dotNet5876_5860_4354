using DalApi;
using BO;
using System;
using DalTest;
using DO = DO; // It was "DO = DalFaced.DO" but it is not rellevent anymore couse of the reference

namespace Helpers;

/// <summary>
/// Manages administrative functions including clock management, configuration settings,
/// and database initialization/reset operations.
/// </summary>
/// <remarks>
/// This static class provides system-wide administrative capabilities such as managing
/// the application clock, system configuration, and database state management. It also
/// includes event notifications for configuration and clock updates.
/// </remarks>
internal static class AdminManager
{
    /// <summary>
    /// Gets the Data Access Layer (DAL) factory instance for accessing data operations.
    /// </summary>
    private static readonly IDal s_dal = DalApi.Factory.Get;

    /// <summary>
    /// Event raised when configuration settings are updated.
    /// </summary>
    /// <remarks>Subscribers can use this event to react to configuration changes in the system.</remarks>
    public static event Action? ConfigUpdatedObservers;

    /// <summary>
    /// Event raised when the system clock is updated.
    /// </summary>
    /// <remarks>Subscribers can use this event to react to clock adjustments in the system.</remarks>
    public static event Action? ClockUpdatedObservers;

    #region Clock Management

    /// <summary>
    /// Gets the current system clock time.
    /// </summary>
    /// <value>A DateTime representing the current application time.</value>
    /// <remarks>The clock is retrieved from the DAL configuration and represents
    /// the system's internal time that may differ from real time for testing purposes.</remarks>
    internal static DateTime Now { get => s_dal.Config.Clock; }

    /// <summary>
    /// Updates the system clock to a new time value.
    /// </summary>
    /// <param name="newClock">The new DateTime value to set as the system clock.</param>
    /// <remarks>This method updates the clock in the DAL configuration and triggers
    /// the ClockUpdatedObservers event to notify all subscribers of the change.</remarks>
    internal static void UpdateClock(DateTime newClock)
    {
        var oldClock = s_dal.Config.Clock;
        s_dal.Config.Clock = newClock;

        ClockUpdatedObservers?.Invoke();
    }

    #endregion

    #region Config Management

    /// <summary>
    /// Retrieves the current system configuration settings.
    /// </summary>
    /// <returns>A Config object containing all current system configuration parameters.</returns>
    /// <remarks>This method reads all configuration values from the DAL and returns them
    /// as a business logic Config object, mapping all clock, manager, courier, and delivery parameters.</remarks>
    internal static BO.Config GetConfig()
    => new BO.Config()
    {
        Clock = s_dal.Config.Clock,
        ManagerId = s_dal.Config.ManagerId,
        ManagerPassword = s_dal.Config.ManagerPassword,
        CompanyAddress = s_dal.Config.CompanyAddress,
        Latitude = s_dal.Config.Latitude,
        Longitude = s_dal.Config.Longitude,
        MaxDeliveryRange = s_dal.Config.MaxDeliveryRange,
        AvgSpeedKmhForCar = s_dal.Config.AvgSpeedKmhForCar,
        AvgSpeedKmhForMotorcycle = s_dal.Config.AvgSpeedKmhForMotorcycle,
        AvgSpeedKmhForBicycle = s_dal.Config.AvgSpeedKmhForBicycle,
        AvgSpeedKmhForFoot = s_dal.Config.AvgSpeedKmhForFoot,
        MaxTimeRangeForDelivery = s_dal.Config.MaxTimeRangeForDelivery,
        RiskRange = s_dal.Config.RiskRange,
        InactivityRange = s_dal.Config.InactivityRange,
    };

    /// <summary>
    /// Updates the system configuration settings with new values.
    /// </summary>
    /// <param name="configuration">A Config object containing the new configuration values to apply.</param>
    /// <remarks>This method compares the new configuration with the current values and only
    /// updates those that have changed. It triggers the ConfigUpdatedObservers event if any
    /// changes are detected. Clock updates trigger the ClockUpdatedObservers event separately.</remarks>
    internal static void SetConfig(BO.Config configuration)
    {
        bool configChanged = false;


        if (s_dal.Config.ManagerId != configuration.ManagerId)
        {
            s_dal.Config.ManagerId = configuration.ManagerId;
            configChanged = true;
        }

        if (s_dal.Config.ManagerPassword != configuration.ManagerPassword)
        {
            // Validate and hash new manager password if provided
            if (!string.IsNullOrEmpty(configuration.ManagerPassword))
            {
                if (!Tools.IsStrongPassword(configuration.ManagerPassword))
                    throw new BO.BlDataValidationException("Password is not strong enough. Must contain at least 8 characters with uppercase, lowercase, digit, and special character.");
                
                s_dal.Config.ManagerPassword = Tools.HashPassword(configuration.ManagerPassword);
            }
            configChanged = true;
        }

        if (s_dal.Config.MaxDeliveryRange != configuration.MaxDeliveryRange)
        {
            s_dal.Config.MaxDeliveryRange = configuration.MaxDeliveryRange;
            configChanged = true;
        }

        if (s_dal.Config.CompanyAddress != configuration.CompanyAddress)
        {
            s_dal.Config.CompanyAddress = configuration.CompanyAddress;
            if (!string.IsNullOrEmpty(configuration.CompanyAddress))
            {
                try
                {
                    var coords = Tools.GetCoordinatesForAddress(configuration.CompanyAddress);
                    s_dal.Config.Latitude = coords.Latitude;
                    s_dal.Config.Longitude = coords.Longitude;
                }
                catch (BO.BlDataValidationException ex)
                {
                    // Revert address change and re-throw validation exception
                    s_dal.Config.CompanyAddress = configuration.CompanyAddress;
                    throw new BO.BlDataValidationException($"Failed to get coordinates for address: {ex.Message}");
                }
            }
            configChanged = true;
        }

        if (s_dal.Config.Latitude != configuration.Latitude)
        {
            s_dal.Config.Latitude = configuration.Latitude;
            configChanged = true;
        }

        if (s_dal.Config.Longitude != configuration.Longitude)
        {
            s_dal.Config.Longitude = configuration.Longitude;
            configChanged = true;
        }

        if (s_dal.Config.AvgSpeedKmhForCar != configuration.AvgSpeedKmhForCar)
        {
            s_dal.Config.AvgSpeedKmhForCar = configuration.AvgSpeedKmhForCar;
            configChanged = true;
        }

        if (s_dal.Config.AvgSpeedKmhForMotorcycle != configuration.AvgSpeedKmhForMotorcycle)
        {
            s_dal.Config.AvgSpeedKmhForMotorcycle = configuration.AvgSpeedKmhForMotorcycle;
            configChanged = true;
        }

        if (s_dal.Config.AvgSpeedKmhForBicycle != configuration.AvgSpeedKmhForBicycle)
        {
            s_dal.Config.AvgSpeedKmhForBicycle = configuration.AvgSpeedKmhForBicycle;
            configChanged = true;
        }

        if (s_dal.Config.AvgSpeedKmhForFoot != configuration.AvgSpeedKmhForFoot)
        {
            s_dal.Config.AvgSpeedKmhForFoot = configuration.AvgSpeedKmhForFoot;
            configChanged = true;
        }

        if (s_dal.Config.MaxTimeRangeForDelivery != configuration.MaxTimeRangeForDelivery)
        {
            s_dal.Config.MaxTimeRangeForDelivery = configuration.MaxTimeRangeForDelivery;
            configChanged = true;
        }

        if (s_dal.Config.RiskRange != configuration.RiskRange)
        {
            s_dal.Config.RiskRange = configuration.RiskRange;
            configChanged = true;
        }

        if (s_dal.Config.InactivityRange != configuration.InactivityRange)
        {
            s_dal.Config.InactivityRange = configuration.InactivityRange;
            configChanged = true;
        }

        if (s_dal.Config.Clock != configuration.Clock)
        {
            UpdateClock(configuration.Clock);
            configChanged = true;
        }

        if (configChanged)
        {
            ConfigUpdatedObservers?.Invoke();
        }
    }

    #endregion

    #region Database Initialization/Reset

    /// <summary>
    /// Resets the database to its initial empty state and resets the system clock to the current time.
    /// </summary>
    /// <remarks>This operation removes all data from the system and restores default configuration values.
    /// The system clock is set to the current real time after the reset.</remarks>
    internal static void ResetDB()
    {
        s_dal.ResetDB();
        UpdateClock(DateTime.Now);
        ConfigUpdatedObservers?.Invoke();
    }

    /// <summary>
    /// Initializes the database with default test data and updates the system clock.
    /// </summary>
    /// <remarks>This operation populates the database with initial data through the Initialization module
    /// and synchronizes the system clock with the configuration's clock value.</remarks>
    internal static void InitializeDB()
    {

        Initialization.Do();

        UpdateClock(s_dal.Config.Clock);
        ConfigUpdatedObservers?.Invoke();
    }

    #endregion
}