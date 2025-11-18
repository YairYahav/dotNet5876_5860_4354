namespace BlImplementation;
using BlApi;
using BO;
using System;
using Helpers;

internal class AdminImplementation : IAdmin
{
    public void ResetDB()
    {
        AdminManager.ResetDB();
    }

    public void InitializeDB()
    {
        AdminManager.InitializeDB();
    }


    public DateTime GetClock()
    {
        return AdminManager.Now;
    }

    public void ForwardClock(TimeUnit unit)
    {
        DateTime newClock = unit switch
        {
            TimeUnit.MINUTE => AdminManager.Now.AddMinutes(1),
            TimeUnit.HOUR => AdminManager.Now.AddHours(1),
            TimeUnit.DAY => AdminManager.Now.AddDays(1),
            TimeUnit.YEAR => AdminManager.Now.AddYears(1),
            _ => AdminManager.Now
        };

        AdminManager.UpdateClock(newClock);
    }

    public Config GetConfig()
    {
        return AdminManager.GetConfig();
    }

    public void SetConfig(Config config)
    {
        AdminManager.SetConfig(config);
    }
}