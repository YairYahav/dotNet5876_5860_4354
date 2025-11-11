namespace Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using DO;





internal static class Config
{

    internal const string s_data_config_xml = "data-config.xml";
    internal const string s_couriers_xml = "couriers.xml";
    internal const string s_orders_xml = "orders.xml";
    internal const string s_deliveries_xml = "deliveries.xml";


    internal static int NextCourierId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, nameof(NextCourierId));
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, nameof(NextCourierId), value);
    }


    internal static int NextOrderId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, nameof(NextOrderId));
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, nameof(NextOrderId), value);
    }


    internal static int NextDeliveryId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, nameof(NextDeliveryId));
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, nameof(NextDeliveryId), value);
    }


    internal static int ManagerId
    {
        get => int.Parse(XMLTools.LoadListFromXMLElement(s_data_config_xml).Element(nameof(ManagerId))?.Value ?? "0");
        set => XMLTools.SetConfigIntVal(s_data_config_xml, nameof(ManagerId), value);
    }


    internal static string ManagerPassword
    {
        get
        {
            var root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            return (string?)root.Element(nameof(ManagerPassword)) ?? string.Empty;
        }
        set
        {
            var root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            root.SetElementValue(nameof(ManagerPassword), value);
            XMLTools.SaveListToXMLElement(root, s_data_config_xml);
        }
    }


    internal static string? CompanyAddress
    {
        get
        {
            var root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            return (string?)root.Element(nameof(CompanyAddress));
        }
        set
        {
            var root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            root.SetElementValue(nameof(CompanyAddress), value);
            XMLTools.SaveListToXMLElement(root, s_data_config_xml);
        }
    }


    internal static double? Latitude
    {
        get
        {
            var root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            return root.ToDoubleNullable(nameof(Latitude));
        }
        set
        {
            var root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            root.SetElementValue(nameof(Latitude), value.HasValue ? value.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : null);
            XMLTools.SaveListToXMLElement(root, s_data_config_xml);
        }
    }

    internal static double? Longitude
    {
        get
        {
            var root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            return root.ToDoubleNullable(nameof(Longitude));
        }
        set
        {
            var root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            root.SetElementValue(nameof(Longitude), value.HasValue ? value.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : null);
            XMLTools.SaveListToXMLElement(root, s_data_config_xml);
        }
    }

    internal static double? MaxDeliveryRange
    {
        get
        {
            var root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            return root.ToDoubleNullable(nameof(MaxDeliveryRange));
        }
        set
        {
            var root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            root.SetElementValue(nameof(MaxDeliveryRange), value.HasValue ? value.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : null);
            XMLTools.SaveListToXMLElement(root, s_data_config_xml);
        }
    }

    internal static double AvgSpeedKmhForCar
    {
        get
        {
            var root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            return root.ToDoubleNullable(nameof(AvgSpeedKmhForCar)) ?? 0.0;
        }
        set
        {
            var root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            root.SetElementValue(nameof(AvgSpeedKmhForCar), value.ToString(System.Globalization.CultureInfo.InvariantCulture));
            XMLTools.SaveListToXMLElement(root, s_data_config_xml);
        }
    }

    internal static double AvgSpeedKmhForMotorcycle
    {
        get
        {
            var root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            return root.ToDoubleNullable(nameof(AvgSpeedKmhForMotorcycle)) ?? 0.0;
        }
        set
        {
            var root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            root.SetElementValue(nameof(AvgSpeedKmhForMotorcycle), value.ToString(System.Globalization.CultureInfo.InvariantCulture));
            XMLTools.SaveListToXMLElement(root, s_data_config_xml);
        }
    }

    internal static double AvgSpeedKmhForBicycle
    {
        get
        {
            var root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            return root.ToDoubleNullable(nameof(AvgSpeedKmhForBicycle)) ?? 0.0;
        }
        set
        {
            var root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            root.SetElementValue(nameof(AvgSpeedKmhForBicycle), value.ToString(System.Globalization.CultureInfo.InvariantCulture));
            XMLTools.SaveListToXMLElement(root, s_data_config_xml);
        }
    }

    internal static double AvgSpeedKmhForFoot
    {
        get
        {
            var root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            return root.ToDoubleNullable(nameof(AvgSpeedKmhForFoot)) ?? 0.0;
        }
        set
        {
            var root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            root.SetElementValue(nameof(AvgSpeedKmhForFoot), value.ToString(System.Globalization.CultureInfo.InvariantCulture));
            XMLTools.SaveListToXMLElement(root, s_data_config_xml);
        }
    }

    internal static TimeSpan MaxTimeRangeForDelivery
    {
        get
        {
            var root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            var s = (string?)root.Element(nameof(MaxTimeRangeForDelivery));
            return TimeSpan.TryParse(s, out var t) ? t : TimeSpan.Zero;
        }
        set
        {
            var root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            root.SetElementValue(nameof(MaxTimeRangeForDelivery), value.ToString());
            XMLTools.SaveListToXMLElement(root, s_data_config_xml);
        }
    }

    internal static TimeSpan RiskRange
    {
        get
        {
            var root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            var s = (string?)root.Element(nameof(RiskRange));
            return TimeSpan.TryParse(s, out var t) ? t : TimeSpan.Zero;
        }
        set
        {
            var root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            root.SetElementValue(nameof(RiskRange), value.ToString());
            XMLTools.SaveListToXMLElement(root, s_data_config_xml);
        }
    }

    internal static TimeSpan InactivityRange
    {
        get
        {
            var root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            var s = (string?)root.Element(nameof(InactivityRange));
            return TimeSpan.TryParse(s, out var t) ? t : TimeSpan.Zero;
        }
        set
        {
            var root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            root.SetElementValue(nameof(InactivityRange), value.ToString());
            XMLTools.SaveListToXMLElement(root, s_data_config_xml);
        }
    }

    internal static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(s_data_config_xml, nameof(Clock));
        set => XMLTools.SetConfigDateVal(s_data_config_xml, nameof(Clock), value);
    }


    internal static void Reset()
    {
        // set some reasonable starting values
        NextCourierId = 200000001;
        NextOrderId = 300000001;
        NextDeliveryId = 400000001;

        ManagerId = 329164354;
        ManagerPassword = "Admin1234";
        CompanyAddress = "Yehuda Hanasi St, 95, Elad";

        Latitude = 32.0512;
        Longitude = 34.8780;
        MaxDeliveryRange = 100;

        AvgSpeedKmhForCar = 90;
        AvgSpeedKmhForMotorcycle = 60;
        AvgSpeedKmhForBicycle = 25;
        AvgSpeedKmhForFoot = 5;

        MaxTimeRangeForDelivery = new TimeSpan(2, 0, 0);
        RiskRange = new TimeSpan(0, 30, 0);
        InactivityRange = new TimeSpan(0, 15, 0);

        Clock = DateTime.Now;
    }
}
