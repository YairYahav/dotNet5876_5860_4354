namespace DalTest;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Handles the initialization of all system data for testing purposes.
/// This class populates the in-memory database (DataSource) with example data 
/// for configuration, couriers, orders, and deliveries.
/// </summary>
public static class Initialization
{
    // Stage 1: (ctrl+k then ctrl+c) ----- old
    //private static ICourier? s_dalCourier;// Access to courier data
    //private static IDelivery? s_dalDelivery;// Access to delivery data
    //private static IOrder? s_dalOrder;// Access to order data
    //private static IConfig? s_dalConfig;// Access to configuration data




    // Stage 2: unified DAL field (replace multiple fields from stage 1)
    private static IDal? s_dal;

    private static readonly Random s_rand = new();// Random number generator for data generation



    /// <summary>
    /// Creates and inserts a set of couriers with randomized but logical attributes.
    /// </summary>
    private static void createCouriers()
    {
        int count = 25;
        string[] firstName = { "John", "Jane", "Michael", "Emily", "David", "Sarah", "Daniel", "Olivia", "James", "Sophia",
                               "Robert", "Isabella", "William", "Mia", "Joseph", "Charlotte", "Charles", "Amelia",
                               "Thomas", "Harper", "Christopher", "Evelyn", "Matthew", "Abigail", "Anthony" };
        string[] lastName = { "Smith", "Johnson", "Brown", "Taylor", "Anderson", "Thomas", "Jackson", "White", "Harris", "Martin","Thompson", "Garcia", "Martinez", "Robinson",
                              "Clark", "Rodriguez", "Lewis", "Lee", "Walker", "Hall", "Allen", "Young", "King", "Wright", "Scott" };
        string domain = "@gmail.com";
        var kind = new[] { DeliveryType.Car, DeliveryType.Motorcycle, DeliveryType.Bicycle, DeliveryType.OnFoot };// Possible delivery types
        for (int i = 0; i < count; i++)
        {
            int id;
            // ensure unique id using DAL via s_dal
            do id = s_rand.Next(200000000, 400000000);
            while (s_dal!.Courier.Read(id) != null); // s_dal instead of s_dalCouriers

            string fullName = firstName[s_rand.Next(firstName.Length)] + " " +
                  lastName[s_rand.Next(lastName.Length)];// Generate full name
            string email = firstName[i % firstName.Length].ToLower() + domain;//Generate email (use modulo to avoid overflow)
            int password = s_rand.Next(1000, 9999);// Generate a simple numeric password
/*            string passwordStr = HashPasswordForSeed(password.ToString());*/// Convert password to string
            string passwordStr = password.ToString();
            bool isActive = s_rand.Next(0, 10) != 0;// 90% chance of being active
            double? CompanyMaxRange = s_dal!.Config.MaxDeliveryRange;// Get company max delivery range
            double? PersonalMaxRange = null;// Personal max range for the courier
            DeliveryType deliveryType = kind[s_rand.Next(kind.Length)];// Randomly assign a delivery type
            if (CompanyMaxRange != null && s_rand.Next(0, 3) == 0)// 33% chance to set a personal max range
            {
                double upper = deliveryType switch// Set upper limit based on delivery type
                {
                    DeliveryType.Car => 30,
                    DeliveryType.Motorcycle => 20,
                    DeliveryType.Bicycle => 10,
                    DeliveryType.OnFoot => 5,
                    _ => 0
                };
                if (CompanyMaxRange > upper)
                    PersonalMaxRange = s_rand.NextDouble() * (upper - 1) + 1;// Set personal max range within upper limit
                else
                    PersonalMaxRange = s_rand.NextDouble() * (CompanyMaxRange.Value - 1) + 1;// Set personal max range within company limit
            }

            DateTime systemClock = s_dal!.Config.Clock;// Get current system clock
            DateTime StartDate = systemClock.AddYears(-2);// Start date for employment
            int rangeInDays = (systemClock - StartDate).Days;// Calculate range in days for employment start date
            DateTime employmentStartDate = StartDate.AddDays(s_rand.Next(rangeInDays));// Randomly assign employment start date within the last 2 years

            // create courier and insert via unified DAL
            s_dal!.Courier.Create(new Courier(
                id,
                fullName,
                "05" + s_rand.Next(100000000, 999999999).ToString(),
                email,
                passwordStr,
                isActive,
                deliveryType,
                employmentStartDate,
                PersonalMaxRange));
        }
    }
    //private static string HashPasswordForSeed(string password)
    //{
    //    using var sha = SHA256.Create();
    //    byte[] bytes = Encoding.UTF8.GetBytes(password);
    //    byte[] hash = sha.ComputeHash(bytes);
    //    return Convert.ToHexString(hash);
    //}

    /// <summary>
    /// Creates and inserts a set of orders with realistic data, addresses, and timestamps.
    /// </summary>
    private static void createOrders()
    {
        var addresses = new (string Address, double Lat, double Lon)[]
            {
            ("7 HaNesi'im St, Petah Tikva, Israel", 32.0880, 34.8870),
            ("10 Jaffa St, Jerusalem, Israel", 31.7839, 35.2160),
            ("50 Allenby St, Tel Aviv, Israel", 32.0683, 34.7743),
            ("3 HaAtzmaut Blvd, Haifa, Israel", 32.8190, 34.9980),
            ("25 Herzl St, Ramat Gan, Israel", 32.0820, 34.8140),
            ("12 Gaza St, Jerusalem, Israel", 31.7773, 35.2102),
            ("120 Ibn Gabirol St, Tel Aviv, Israel", 32.0852, 34.7821),
            ("90 Jabotinsky St, Bnei Brak, Israel", 32.0960, 34.8430),
            ("8 Ben Yehuda St, Jerusalem, Israel", 31.7801, 35.2204),
            ("14 Dizengoff St, Tel Aviv, Israel", 32.0770, 34.7742),
            ("22 Weizmann St, Herzliya, Israel", 32.1663, 34.8353),
            ("18 Yehuda St, Netanya, Israel", 32.3291, 34.8530),
            ("45 Rothschild Blvd, Tel Aviv, Israel", 32.0625, 34.7720),
            ("28 Emek Refaim St, Jerusalem, Israel", 31.7620, 35.2192),
            ("2 HaShalom Rd, Ramat Gan, Israel", 32.0705, 34.8101),
            ("8 Pinsker St, Tel Aviv, Israel", 32.0740, 34.7732),
            ("19 King George St, Jerusalem, Israel", 31.7836, 35.2170),
            ("27 Ahad Ha’am St, Tel Aviv, Israel", 32.0620, 34.7733),
            ("6 Ben Gurion Blvd, Herzliya, Israel", 32.1621, 34.8072),
            ("23 Begin St, Beersheba, Israel", 31.2522, 34.7915)
            };

        string[] customerNames ={"Rina Azulay","Moshe Peretz","Dana Levi","Avi Cohen","Noa Biton","Tamar Lavi","Eli Amar","Shai Ben Haim","Itay Shalem","Hila Regev",
                                  "Liad Cohen","Maya Levi","Yair Azulay","Shira Ron","Omer Katz","Gal Turner","Yuval Harel","Neta Alon","Eden Ben David","Tomer Ashkenazi"};

        string[] customerPhones ={"0501234567","0527654321","0549876543","0532223344","0507654321","0523332222","0541112223","0509998887","0524445566","0545556667",
                                    "0503216549","0531112233","0542233445","0529090909","0508080808","0547001200","0526003300","0501122334","0534455667","0548889990"};

        var descPool = new[] { "Office supplies", "Fragile glassware", "Books", "Electronics", "Clothing", "Food package", "Medicine", "Documents", "Small furniture", "Gifts" };// Possible order descriptions
        var types = new[] { OrderType.Regular, OrderType.Heavy, OrderType.Fragile, OrderType.Refrigerated, OrderType.Express };// Possible order types
        for (int i = 0; i < 50; i++)
        {
            var pick = addresses[s_rand.Next(addresses.Length)];// Randomly select an address
            string cusName = customerNames[s_rand.Next(customerNames.Length)];// Randomly select a customer name
            string cusPhone = customerPhones[s_rand.Next(customerPhones.Length)];// Randomly select a customer phone number
            string desc = descPool[s_rand.Next(descPool.Length)];// Randomly select an order description
            var now = s_dal!.Config.Clock;// Get current system clock
            int days = s_rand.Next(0, 30);// Randomly select days within the last month
            int hours = s_rand.Next(0, 24);// Randomly select hours
            int minutes = s_rand.Next(0, 60);// Randomly select hours and minutes
            DateTime orderTime = now.AddDays(-days).AddHours(-hours).AddMinutes(-minutes);// Calculate order placement time
            OrderType orderType = types[s_rand.Next(types.Length)];// Randomly select an order type
            bool isFragile = orderType == OrderType.Fragile;// Set fragile flag based on order type
            double? volume = s_rand.Next(1, 100);// Randomly assign a volume
            double? weight = s_rand.Next(1, 50);// Randomly assign a weight

            s_dal!.Order.Create(new Order(
                0,
                orderType,
                pick.Address,
                pick.Lat,
                pick.Lon,
                cusName,
                cusPhone,
                isFragile,
                orderTime,
                volume,
                weight,
                desc));
        }
    }



    /// <summary>
    /// Creates and inserts delivery records linking couriers and orders.
    /// Ensures logical and valid relations between entities.
    /// </summary>
    private static void createDeliveries()
    {
        var orders = s_dal!.Order.ReadAll().ToList();// Get all orders
        var couriers = s_dal!.Courier.ReadAll().Where(c => c.IsActive).ToList();// Get all active couriers
        if (orders.Count == 0 || couriers.Count == 0)// No orders or couriers available
            return;

        int openOrders = 20; // Number of open orders to create
        int closeOrders = 20; // Number of closed orders to create
        int inProgressOrders = 10; // Number of in-progress orders to create
        var endKind = new[] {
            TypeOfDeliveryCompletionTime.Supplied,
            TypeOfDeliveryCompletionTime.Failed,
            TypeOfDeliveryCompletionTime.Canceled,
            TypeOfDeliveryCompletionTime.RefusedByCustomer,
            TypeOfDeliveryCompletionTime.CustomerNotFound
        }; // Possible delivery completion types

        double? companyLat = s_dal!.Config.Latitude, companyLon = s_dal!.Config.Longitude;
        double? companyMaxRange = s_dal!.Config.MaxDeliveryRange;

        /// <summary>
        /// Create in-progress deliveries
        /// </summary>
        for (int i = 0; i < openOrders && i < orders.Count; i++)
        {
            var order = TakeRandomOrder(orders);// Take a random order
            var courier = PickEligibleCourier(order, couriers, companyLat ?? 0, companyLon ?? 0, companyMaxRange);// Pick an eligible courier for the order
            if (courier == null) continue;
            DateTime start = RandomStartAfter((DateTime)order.OrderPlacementTime, 12);

            s_dal!.Delivery.Create(new Delivery(
                0,
                order.Id,
                courier.Id,
                start,
                courier.DeliveryType,
                HaversineKm(companyLat ?? 0, companyLon ?? 0, order.Latitude, order.Longitude),
                null,
                null));// Create and insert the delivery record
        }

        /// <summary>
        /// Create closed deliveries
        /// </summary>
        for (int i = 0; i < closeOrders && i < orders.Count; i++)
        {
            var order = TakeRandomOrder(orders);
            var courier = PickEligibleCourier(order, couriers, companyLat ?? 0, companyLon ?? 0, companyMaxRange);
            if (courier == null) continue;
            DateTime start = RandomStartAfter((DateTime)order.OrderPlacementTime, 24);
            DateTime end = start.AddMinutes(s_rand.Next(25, 180));
            var endType = endKind[s_rand.Next(endKind.Length)];
            s_dal!.Delivery.Create(new Delivery(
                0,
                order.Id,
                courier.Id,
                start,
                courier.DeliveryType,
                HaversineKm(companyLat ?? 0, companyLon ?? 0, order.Latitude, order.Longitude),
                end,
                endType));
            
            // Remove order from list if it was successfully delivered (not Failed or CustomerNotFound)
            if (endType != TypeOfDeliveryCompletionTime.Failed && 
                endType != TypeOfDeliveryCompletionTime.CustomerNotFound)
            {
                // Order is already removed by TakeRandomOrder, so no action needed
                // This ensures we don't re-assign the same order again
            }
        }
    }


    private static Courier? PickEligibleCourier(Order order, List<Courier> couriers, double compabyLat, double companyLon, double? companyMax)
    {
        double airKm = HaversineKm(compabyLat, companyLon, order.Latitude, order.Longitude);
        var eligible = couriers.Where(c =>
            (companyMax is null || airKm <= companyMax.Value) &&
            (c.MaxPersonalDeliveryDistance is null || airKm <= c.MaxPersonalDeliveryDistance.Value)
        ).ToList();

        return eligible.Count == 0 ? null : eligible[s_rand.Next(eligible.Count)];
    }

    /// <summary>
    /// Generates a random <see cref="DateTime"/> that occurs after the specified time, within a given range of hours.
    /// </summary>
    /// <remarks>The method ensures that the returned time is not earlier than the current time. If the
    /// generated time is earlier than the current time, a fallback value of 5 minutes after the <paramref
    /// name="after"/> parameter is returned.</remarks>
    /// <param name="after">The <see cref="DateTime"/> after which the random time should be generated.</param>
    /// <param name="maxHours">The maximum number of hours to add to the <paramref name="after"/> parameter. Must be non-negative.</param>
    /// <returns>A random <see cref="DateTime"/> that is either within the specified range or, if the generated time is in the
    /// past, a fallback time 5 minutes after the <paramref name="after"/> parameter.</returns>
    private static DateTime RandomStartAfter(DateTime after, int maxHours)
    {
        var now = s_dal!.Config.Clock;
        var candidate = after.AddHours(s_rand.Next(0, maxHours + 1)).AddMinutes(s_rand.Next(0, 60));
        return candidate > now ? candidate : after.AddMinutes(5);
    }

    /// <summary>
    /// Removes and returns a random <see cref="Order"/> from the specified list.
    /// </summary>
    /// <param name="list">The list of <see cref="Order"/> objects to select from. Must not be null or empty.</param>
    /// <returns>A randomly selected <see cref="Order"/> from the list.</returns>
    private static Order TakeRandomOrder(List<Order> list)
    {
        int i = s_rand.Next(list.Count);
        var x = list[i];
        list.RemoveAt(i);
        return x;
    }

    /// <summary>
    /// Calculates the Haversine distance in kilometers between two geographic coordinates.
    /// </summary>
    /// <param name="lat1"></param>
    /// <param name="lon1"></param>
    /// <param name="lat2"></param>
    /// <param name="lon2"></param>
    /// <returns></returns>
    /// 
    private static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371.0;
        static double ToRad(double deg) => deg * Math.PI / 180.0;

        double dLat = ToRad(lat2 - lat1);
        double dLon = ToRad(lon2 - lon1);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    /// <summary>
    /// Creates and sets the configuration parameters for the DAL.
    /// </summary>
    static void createConfig()
    {
        s_dal!.Config.ManagerId = 329164354;
        s_dal!.Config.ManagerPassword = "Admin1234";
        s_dal!.Config.CompanyAddress = "Yehuda Hanasi St, 95 , Elad ";
        s_dal!.Config.Latitude = 32.0512;
        s_dal!.Config.Longitude = 34.8780;
        s_dal!.Config.MaxDeliveryRange = 100;
        s_dal!.Config.AvgSpeedKmhForCar = 90;
        s_dal!.Config.AvgSpeedKmhForMotorcycle = 60;
        s_dal!.Config.AvgSpeedKmhForBicycle = 25;
        s_dal!.Config.AvgSpeedKmhForFoot = 5;
        s_dal!.Config.MaxTimeRangeForDelivery = new TimeSpan(2, 0, 0);
        s_dal!.Config.RiskRange = new TimeSpan(0, 30, 0);
        s_dal!.Config.InactivityRange = new TimeSpan(0, 15, 0);
    }
    
    
    
    
    
    /// <param name="dal">Unified DAL implementation (e.g., new DalList())</param>
    public static void Do()
    {
        // assign the provided dal to the static field (and validate not null)
        s_dal = DalApi.Factory.Get;

        Console.WriteLine("Reset configuration and lists...");
        // now we can reset everything through one method
        s_dal.ResetDB();

        Console.WriteLine("Initializing data...");
        createConfig();// Set configuration parameters
        createCouriers();// Populate couriers
        createOrders();// Populate orders
        createDeliveries();// Populate deliveries
        Console.WriteLine("Initialization completed");
    }
}
