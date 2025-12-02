using System;
using BlApi;
using BO;

internal class Program
{
    private static readonly IBl s_bl = BlApi.Factory.Get();

    private static int RequesterId;
    private static UserRole LoggedRole;

    private enum MainMenu
    {
        Exit = 0,
        Admin = 1,
        Courier = 2,
        Orders = 3
    }

    private enum AdminMenu
    {
        Back = 0,
        ResetDB = 1,
        InitializeDB = 2,
        GetClock = 3,
        ForwardClock = 4,
        GetConfig = 5,
        SetConfig = 6
    }

    private enum CourierMenu
    {
        Back = 0,
        ListCouriers = 1,
        GetCourier = 2,
        CreateCourier = 3,
        UpdateCourier = 4,
        DeleteCourier = 5
    }

    private enum OrderMenu
    {
        Back = 0,
        Summary = 1,
        ListOrders = 2,
        ListClosedOrders = 3,
        ListOpenOrders = 4,
        GetOrder = 5,
        CreateOrder = 6,
        UpdateOrder = 7,
        CancelOrder = 8,
        DeleteOrder = 9,
        CompleteOrder = 10,
        ChooseOrder = 11
    }

    private static void Main()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("===== BL TEST =====");
        Console.ResetColor();

        LoginScreen();

        while (true)
        {
            switch (ShowMainMenu())
            {
                case MainMenu.Exit:
                    return;

                case MainMenu.Admin:
                    if (LoggedRole != UserRole.Admin)
                    {
                        Console.WriteLine("Access only for admin");
                        break;
                    }
                    AdminSubMenu();
                    break;

                case MainMenu.Courier:
                    CourierSubMenu();
                    break;

                case MainMenu.Orders:
                    OrdersSubMenu();
                    break;
            }
        }
    }

    // ===================== LOGIN =====================

    private static void LoginScreen()
    {
        Console.WriteLine("Login to system");

        while (true)
        {
            Console.Write("Enter ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID");
                continue;
            }

            Console.Write("Enter password: ");
            string? pass = Console.ReadLine() ?? "";

            try
            {
                LoggedRole = s_bl.Courier.Login(id, pass);
                RequesterId = id;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Logged as {LoggedRole}");
                Console.ResetColor();
                return;
            }
            catch (Exception ex)
            {
                PrintException(ex);
            }
        }
    }

    private static MainMenu ShowMainMenu()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n=== MAIN MENU ===");
        Console.ResetColor();

        Console.WriteLine("1. Admin");
        Console.WriteLine("2. Courier");
        Console.WriteLine("3. Orders");
        Console.WriteLine("0. Exit");

        Console.Write("Choose: ");
        if (!Enum.TryParse(Console.ReadLine(), out MainMenu option))
            option = MainMenu.Exit;

        return option;
    }

    // ===================== ADMIN =====================

    private static void AdminSubMenu()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n=== ADMIN MENU ===");
            Console.ResetColor();

            Console.WriteLine("1. Reset DB");
            Console.WriteLine("2. Initialize DB");
            Console.WriteLine("3. Get Clock");
            Console.WriteLine("4. Forward Clock");
            Console.WriteLine("5. Get Config");
            Console.WriteLine("6. Set Config");
            Console.WriteLine("0. Back");

            Console.Write("Choose: ");
            Enum.TryParse(Console.ReadLine(), out AdminMenu option);

            try
            {
                switch (option)
                {
                    case AdminMenu.Back:
                        return;

                    case AdminMenu.ResetDB:
                        s_bl.Admin.ResetDB();
                        Console.WriteLine("DB Reset");
                        break;

                    case AdminMenu.InitializeDB:
                        s_bl.Admin.InitializeDB();
                        Console.WriteLine("DB Initialized");
                        break;

                    case AdminMenu.GetClock:
                        Console.WriteLine(s_bl.Admin.GetClock());
                        break;

                    case AdminMenu.ForwardClock:
                        ForwardClockMenu();
                        break;

                    case AdminMenu.GetConfig:
                        Console.WriteLine(s_bl.Admin.GetConfig());
                        break;

                    case AdminMenu.SetConfig:
                        SetConfigMenu();
                        break;
                }
            }
            catch (Exception ex)
            {
                PrintException(ex);
            }
        }
    }

    private static void ForwardClockMenu()
    {
        Console.WriteLine("Choose unit:");
        Console.WriteLine("1. Minute");
        Console.WriteLine("2. Hour");
        Console.WriteLine("3. Day");
        Console.WriteLine("4. Year");

        Console.Write("Select: ");
        if (int.TryParse(Console.ReadLine(), out int x)
            && Enum.IsDefined(typeof(TimeUnit), x - 1))
        {
            var unit = (TimeUnit)(x - 1);
            s_bl.Admin.ForwardClock(unit);
            Console.WriteLine("Clock forwarded");
        }
        else
        {
            Console.WriteLine("Invalid unit");
        }
    }

    private static void SetConfigMenu()
    {
        Config cfg = s_bl.Admin.GetConfig();
        Console.WriteLine("Editing config. Leave empty to skip value.");

        Console.Write("MaxRange: ");
        string? s = Console.ReadLine();
        if (int.TryParse(s, out int v))
            cfg.MaxDeliveryRange = v;

        s_bl.Admin.SetConfig(cfg);
        Console.WriteLine("Config updated");
    }

    // ===================== COURIER =====================

    private static void CourierSubMenu()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n=== COURIER MENU ===");
            Console.ResetColor();

            Console.WriteLine("1. List Couriers");
            Console.WriteLine("2. Get Courier");
            Console.WriteLine("3. Create Courier");
            Console.WriteLine("4. Update Courier");
            Console.WriteLine("5. Delete Courier");
            Console.WriteLine("0. Back");

            Console.Write("Choose: ");
            Enum.TryParse(Console.ReadLine(), out CourierMenu menu);

            try
            {
                switch (menu)
                {
                    case CourierMenu.Back:
                        return;

                    case CourierMenu.ListCouriers:
                        ListCouriers();
                        break;

                    case CourierMenu.GetCourier:
                        GetCourier();
                        break;

                    case CourierMenu.CreateCourier:
                        CreateCourier();
                        break;

                    case CourierMenu.UpdateCourier:
                        UpdateCourier();
                        break;

                    case CourierMenu.DeleteCourier:
                        DeleteCourier();
                        break;
                }
            }
            catch (Exception ex)
            {
                PrintException(ex);
            }
        }
    }

    private static void ListCouriers()
    {
        Console.Write("Filter only active? (true/false, empty = null): ");
        string? s = Console.ReadLine();
        bool? onlyActive = null;
        if (bool.TryParse(s, out bool activeVal))
            onlyActive = activeVal;

        Console.WriteLine("Order by (empty = null):");
        Console.WriteLine("1. ById");
        Console.WriteLine("2. ByName");
        Console.WriteLine("3. ByActive");
        Console.WriteLine("4. EmploymentStartDate");
        Console.WriteLine("5. DeliveryType");
        Console.Write("Select: ");
        string? o = Console.ReadLine();
        CourierListOrderBy? orderBy = null;
        if (int.TryParse(o, out int ix) && Enum.IsDefined(typeof(CourierListOrderBy), ix - 1))
            orderBy = (CourierListOrderBy)(ix - 1);

        var list = s_bl.Courier.GetCouriers(RequesterId, onlyActive, orderBy);
        foreach (var c in list)
            Console.WriteLine(c);
    }

    private static void GetCourier()
    {
        Console.Write("Courier ID: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            var c = s_bl.Courier.GetCourier(RequesterId, id);
            Console.WriteLine(c);
        }
        else
        {
            Console.WriteLine("Invalid ID");
        }
    }

    private static void CreateCourier()
    {
        Console.Write("ID: ");
        int courierId = 0;
        if (!int.TryParse(Console.ReadLine(), out courierId))
        {
            Console.WriteLine("Invalid ID");
            return;
        }

        Console.Write("Full name: ");
        string fullName = Console.ReadLine() ?? "";

        Console.Write("Phone: ");
        string phone = Console.ReadLine() ?? "";

        Console.Write("Email: ");
        string email = Console.ReadLine() ?? "";

        Console.Write("Password: ");
        string password = Console.ReadLine() ?? "";

        Console.Write("Active? (true/false): ");
        bool isActive = false;
        if (bool.TryParse(Console.ReadLine(), out bool act))
            isActive = act;

        Console.WriteLine("Delivery type: 1.Car 2.Motorcycle 3.Bicycle 4.OnFoot");
        DeliveryType deliveryType = DeliveryType.Car;
        if (int.TryParse(Console.ReadLine(), out int dt)
            && Enum.IsDefined(typeof(DeliveryType), dt - 1))
            deliveryType = (DeliveryType)(dt - 1);

        Console.Write("Max personal distance: ");
        double maxDist = 0;
        if (double.TryParse(Console.ReadLine(), out double dist))
            maxDist = dist;

        BO.Courier c = new BO.Courier
        {
            Id = courierId,
            FullName = fullName,
            PhoneNumber = phone,
            Gmail = email,
            Password = password,
            IsActive = isActive,
            DeliveryType = deliveryType,
            MaxPersonalDeliveryDistance = maxDist,
            EmploymentStartDate = DateTime.Now
        };

        s_bl.Courier.CreateCourier(RequesterId, c);
        Console.WriteLine("Courier created");
    }

    private static void UpdateCourier()
    {
        Console.Write("Courier ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID");
            return;
        }

        var c = s_bl.Courier.GetCourier(RequesterId, id);

        Console.Write("New name (empty = skip): ");
        string? s = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(s))
            c.FullName = s;

        Console.Write("New phone (empty = skip): ");
        s = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(s))
            c.PhoneNumber = s;

        Console.Write("New email (empty = skip): ");
        s = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(s))
            c.Gmail = s;

        Console.Write("New password (empty = skip): ");
        s = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(s))
            c.Password = s;

        s_bl.Courier.UpdateCourier(RequesterId, c);
        Console.WriteLine("Courier updated");
    }

    private static void DeleteCourier()
    {
        Console.Write("Courier ID: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            s_bl.Courier.DeleteCourier(RequesterId, id);
            Console.WriteLine("Courier deleted");
        }
        else
        {
            Console.WriteLine("Invalid ID");
        }
    }

    // ===================== ORDERS =====================

    private static void OrdersSubMenu()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n=== ORDERS MENU ===");
            Console.ResetColor();

            Console.WriteLine("1. Orders summary");
            Console.WriteLine("2. List orders");
            Console.WriteLine("3. List closed orders of courier");
            Console.WriteLine("4. List open orders of courier");
            Console.WriteLine("5. Get order");
            Console.WriteLine("6. Create order");
            Console.WriteLine("7. Update order");
            Console.WriteLine("8. Cancel order");
            Console.WriteLine("9. Delete order");
            Console.WriteLine("10. Complete order");
            Console.WriteLine("11. Choose order for delivery");
            Console.WriteLine("0. Back");

            Console.Write("Choose: ");
            Enum.TryParse(Console.ReadLine(), out OrderMenu op);

            try
            {
                switch (op)
                {
                    case OrderMenu.Back:
                        return;

                    case OrderMenu.Summary:
                        OrdersSummary();
                        break;

                    case OrderMenu.ListOrders:
                        ListOrders();
                        break;

                    case OrderMenu.ListClosedOrders:
                        ListClosedOrders();
                        break;

                    case OrderMenu.ListOpenOrders:
                        ListOpenOrders();
                        break;

                    case OrderMenu.GetOrder:
                        GetOrder();
                        break;

                    case OrderMenu.CreateOrder:
                        CreateOrder();
                        break;

                    case OrderMenu.UpdateOrder:
                        UpdateOrder();
                        break;

                    case OrderMenu.CancelOrder:
                        CancelOrder();
                        break;

                    case OrderMenu.DeleteOrder:
                        DeleteOrder();
                        break;

                    case OrderMenu.CompleteOrder:
                        CompleteOrder();
                        break;

                    case OrderMenu.ChooseOrder:
                        ChooseOrder();
                        break;
                }
            }
            catch (Exception ex)
            {
                PrintException(ex);
            }
        }
    }

    private static void OrdersSummary()
    {
        var arr = s_bl.Order.GetOrdersSummary(RequesterId);
        Console.WriteLine("Orders summary (index: count):");
        for (int i = 0; i < arr.Length; i++)
            Console.WriteLine($"{i}: {arr[i]}");
    }

    private static void ListOrders()
    {
        var filterBy = AskNullableEnum<OrderListFilterBy>("FilterBy (empty = no filter)");
        object? filterValue = null;

        if (filterBy != null)
        {
            if (filterBy == OrderListFilterBy.ByStatus)
            {
                var st = AskNullableEnum<OrderStatus>("OrderStatus (empty = no filter value)");
                if (st != null)
                    filterValue = st.Value;
            }
            else if (filterBy == OrderListFilterBy.ByTiming)
            {
                var sch = AskNullableEnum<ScheduleStatus>("ScheduleStatus (empty = no filter value)");
                if (sch != null)
                    filterValue = sch.Value;
            }
        }

        var orderBy = AskNullableEnum<OrderListOrderBy>("OrderBy (empty = default)");

        var list = s_bl.Order.GetOrders(RequesterId, filterBy, filterValue, orderBy);
        foreach (var o in list)
            Console.WriteLine(o);
    }

    private static void ListClosedOrders()
    {
        Console.Write("Courier ID: ");
        if (!int.TryParse(Console.ReadLine(), out int cid))
        {
            Console.WriteLine("Invalid ID");
            return;
        }

        var filterBy = AskNullableEnum<OrderType>("FilterBy order type (empty = no filter)");
        var orderBy = AskNullableEnum<ClosedOrdersListOrderBy>("OrderBy (empty = default)");

        var list = s_bl.Order.GerClosedOrders(RequesterId, cid, filterBy, orderBy);
        foreach (var item in list)
            Console.WriteLine(item);
    }

    private static void ListOpenOrders()
    {
        Console.Write("Courier ID: ");
        if (!int.TryParse(Console.ReadLine(), out int cid))
        {
            Console.WriteLine("Invalid ID");
            return;
        }

        var filterBy = AskNullableEnum<OrderType>("FilterBy order type (empty = no filter)");
        var orderBy = AskNullableEnum<OpenDeliveryListOrderBy>("OrderBy (empty = default)");

        var list = s_bl.Order.GetOpenOrders(RequesterId, cid, filterBy, orderBy);
        foreach (var item in list)
            Console.WriteLine(item);
    }

    private static void GetOrder()
    {
        Console.Write("Order ID: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            var o = s_bl.Order.GetOrder(RequesterId, id);
            Console.WriteLine(o);
        }
        else
        {
            Console.WriteLine("Invalid ID");
        }
    }

    private static void CreateOrder()
    {
        Console.WriteLine("Order type: 1.Regular 2.Express 3.Heavy 4.Fragile 5.Refrigerated (empty = Regular)");
        OrderType orderType = OrderType.Regular;
        string? typeInput = Console.ReadLine();
        if (int.TryParse(typeInput, out int x) && Enum.IsDefined(typeof(OrderType), x - 1))
            orderType = (OrderType)(x - 1);

        Console.Write("Address: ");
        string address = Console.ReadLine() ?? "";

        Console.Write("Latitude: ");
        double lat = 0;
        if (double.TryParse(Console.ReadLine(), out double latVal))
            lat = latVal;

        Console.Write("Longitude: ");
        double lon = 0;
        if (double.TryParse(Console.ReadLine(), out double lonVal))
            lon = lonVal;

        Console.Write("Customer name: ");
        string customerName = Console.ReadLine() ?? "";

        Console.Write("Customer phone: ");
        string customerPhone = Console.ReadLine() ?? "";

        Console.Write("Fragile (true/false, empty = false): ");
        bool isFrag = false;
        if (bool.TryParse(Console.ReadLine(), out bool f))
            isFrag = f;

        Console.Write("Volume (double, empty = 0): ");
        double vol = 0;
        if (double.TryParse(Console.ReadLine(), out double volVal))
            vol = volVal;

        Console.Write("Weight (double, empty = 0): ");
        double w = 0;
        if (double.TryParse(Console.ReadLine(), out double wVal))
            w = wVal;

        Console.Write("Description: ");
        string description = Console.ReadLine() ?? "";

        BO.Order o = new BO.Order
        {
            orderType = orderType,
            AddressOfOrder = address,
            Latitude = lat,
            Longitude = lon,
            CustomerName = customerName,
            CustomerPhone = customerPhone,
            IsFrag = isFrag,
            Volume = vol,
            Weight = w,
            DescriptionOfOrder = description,
            OrderPlacementTime = DateTime.Now
        };

        s_bl.Order.CreateOrder(RequesterId, o);
        Console.WriteLine("Order created");
    }

    private static void UpdateOrder()
    {
        Console.Write("Order ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID");
            return;
        }

        var o = s_bl.Order.GetOrder(RequesterId, id);

        Console.Write("New address (empty = skip): ");
        string? s = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(s))
            o.AddressOfOrder = s;

        Console.Write("New description (empty = skip): ");
        s = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(s))
            o.DescriptionOfOrder = s;

        s_bl.Order.UpdateOrder(RequesterId, o);
        Console.WriteLine("Order updated");
    }

    private static void CancelOrder()
    {
        Console.Write("Order ID: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            s_bl.Order.CancelOrder(RequesterId, id);
            Console.WriteLine("Order canceled");
        }
        else
        {
            Console.WriteLine("Invalid ID");
        }
    }

    private static void DeleteOrder()
    {
        Console.Write("Order ID: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            s_bl.Order.DeleteOrder(RequesterId, id);
        }
        else
        {
            Console.WriteLine("Invalid ID");
        }
    }

    private static void CompleteOrder()
    {
        Console.Write("Courier ID: ");
        if (!int.TryParse(Console.ReadLine(), out int cid))
        {
            Console.WriteLine("Invalid courier ID");
            return;
        }

        Console.Write("Delivery ID: ");
        if (!int.TryParse(Console.ReadLine(), out int did))
        {
            Console.WriteLine("Invalid delivery ID");
            return;
        }

        s_bl.Order.CompleteOrderForCourier(RequesterId, cid, did);
        Console.WriteLine("Order completed");
    }

    private static void ChooseOrder()
    {
        Console.Write("Courier ID: ");
        if (!int.TryParse(Console.ReadLine(), out int cid))
        {
            Console.WriteLine("Invalid courier ID");
            return;
        }

        Console.Write("Order ID: ");
        if (!int.TryParse(Console.ReadLine(), out int oid))
        {
            Console.WriteLine("Invalid order ID");
            return;
        }

        s_bl.Order.ChooseOrderForDelivery(RequesterId, cid, oid);
        Console.WriteLine("Order assigned to courier");
    }

    // ===================== HELPERS =====================

    private static TEnum? AskNullableEnum<TEnum>(string title) where TEnum : struct, Enum
    {
        Console.WriteLine($"{title} (empty = null):");

        var values = Enum.GetValues(typeof(TEnum));
        int i = 1;
        foreach (var val in values)
        {
            Console.WriteLine($"{i}. {val}");
            i++;
        }

        Console.Write("Select: ");
        string? s = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(s))
            return null;

        if (int.TryParse(s, out int index) && index >= 1 && index <= values.Length)
        {
            return (TEnum)values.GetValue(index - 1)!;
        }

        Console.WriteLine("Invalid selection, using null");
        return null;
    }

    private static void PrintException(Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\nException:");
        Console.WriteLine(ex.GetType().Name);
        Console.WriteLine(ex.Message);
        if (ex.InnerException != null)
            Console.WriteLine("Inner: " + ex.InnerException.Message);
        Console.ResetColor();
    }
}
