using DalApi;
using BO;
using System;
using DalTest;
using DO = DO; // It was "DO = DalFaced.DO" but it is not rellevent anymore couse of the reference

namespace Helpers;
internal static class AdminManager
{
    private static readonly IDal s_dal = DalApi.Factory.Get;


    #region Clock Management

    internal static DateTime Now { get => s_dal.Config.Clock; }

    internal static void UpdateClock(DateTime newClock)
    {
        var oldClock = s_dal.Config.Clock;
        s_dal.Config.Clock = newClock;

    }

    #endregion

    #region Config Management

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
            s_dal.Config.ManagerPassword = configuration.ManagerPassword;
            configChanged = true;
        }

        if (s_dal.Config.MaxDeliveryRange != configuration.MaxDeliveryRange)
        {
            s_dal.Config.MaxDeliveryRange = configuration.MaxDeliveryRange;
            configChanged = true;
        }


        if (s_dal.Config.Clock != configuration.Clock)
        {
            UpdateClock(configuration.Clock);
            configChanged = true;
        }
    }

    #endregion

    #region Database Initialization/Reset

    internal static void ResetDB()
    {
        s_dal.ResetDB();
        UpdateClock(DateTime.Now);
    }

    internal static void InitializeDB()
    {

        Initialization.Do();

        UpdateClock(s_dal.Config.Clock);
    }

    #endregion
}