using BlApi;
using BO;
using System;
using System.Linq;
using System.Collections.Generic;

namespace BlTest;

internal class Program
{
    static readonly IBl s_bl = BlApi.Factory.Get();

    private enum MainMenuOptions { Exit, Admin, Couriers, Orders, Deliveries }

    private enum SubMenuOptions { Back, Create, Read, ReadAll, Update, Delete }

    private enum AdminMenuOptions { Back, ResetDB, InitializeDB, GetClock, ForwardClock, GetConfig, SetConfig }

    #region --- Helper Methods (Ask Input) ---

    /// <summary> Prompts for an integer and ensures valid input. </summary>
    /// The rest r pretty much the same but asking 4 someting else. 
    private static int AskInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            if (int.TryParse(s, out var v)) return v;
            Console.WriteLine("ERROR: Please enter a valid integer.");
        }
    }


    private static string AskString(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? "";
    }


    private static T AskEnum<T>(string prompt) where T : struct, Enum
    {
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            if (Enum.TryParse<T>(s, out var v)) return v;
            Console.WriteLine($"ERROR: Please enter a valid value from the list or its corresponding number: ({string.Join(", ", Enum.GetNames(typeof(T)))}).");
        }
    }


    private static double? AskNullableDouble(string prompt)
    {
        Console.Write(prompt);
        var s = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(s)) return null;
        if (double.TryParse(s, out var v)) return v;
        Console.WriteLine("WARNING: Invalid input. Value will be set to null.");
        return null;
    }


    private static bool AskBool(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            if (bool.TryParse(s, out var v)) return v;
            Console.WriteLine("ERROR: Please enter 'true' or 'false'.");
        }
    }


    private static DateTime AskDateTime(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            if (DateTime.TryParse(s, out var v)) return v;
            Console.WriteLine("ERROR: Please enter a valid date and time (e.g., 2025-11-20 14:30).");
        }
    }

    #endregion

    #region --- Helper Methods (Print Output) ---


    private static void PrintCollection<T>(IEnumerable<T> collection)
    {
        int count = 0;
        foreach (var item in collection)
        {
            Console.WriteLine(item); // Calls ToString() which uses reflection helper
            count++;
        }
        if (count == 0)
            Console.WriteLine("The list is empty.");
    }


    private static void HandleBlException(Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n--- Business Logic (BL) ERROR ---");
        Console.WriteLine($"Error Type: {ex.GetType().Name}");
        Console.WriteLine($"Message: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Inner (DAL) Exception: {ex.InnerException.GetType().Name} - {ex.InnerException.Message}");
        }
        Console.ResetColor();
    }

    #endregion

    #region --- Menu Handlers ---

    private static MainMenuOptions AskMainMenu()
    {
        Console.WriteLine("\n=== Main Menu ===");
        Console.WriteLine("0) Exit");
        Console.WriteLine("1) Admin Management");
        Console.WriteLine("2) Couriers");
        Console.WriteLine("3) Orders");
        Console.WriteLine("4) Deliveries");
        Console.Write("Select option: ");
        return Enum.TryParse(Console.ReadLine(), out MainMenuOptions choice) ? choice : MainMenuOptions.Exit;
    }

    private static void HandleAdminMenu()
    {
        while (true)
        {
            Console.WriteLine("\n=== Admin Menu ===");
            Console.WriteLine("0) Back to Main Menu");
            Console.WriteLine("1) Reset Database (ResetDB)");
            Console.WriteLine("2) Initialize Database with Dummy Data (InitializeDB)");
            Console.WriteLine("3) Display System Clock");
            Console.WriteLine("4) Forward System Clock");
            Console.WriteLine("5) Display Configuration (Config)");
            Console.WriteLine("6) Update Configuration (Config)");
            Console.Write("Select option: ");
            if (!Enum.TryParse(Console.ReadLine(), out AdminMenuOptions choice)) continue;

            try
            {
                switch (choice)
                {
                    case AdminMenuOptions.Back: return;

                    case AdminMenuOptions.ResetDB:
                        s_bl.Admin.ResetDB();
                        Console.WriteLine("✅ Database successfully reset.");
                        break;

                    case AdminMenuOptions.InitializeDB:
                        s_bl.Admin.InitializeDB();
                        Console.WriteLine("✅ Database successfully initialized with dummy data.");
                        break;

                    case AdminMenuOptions.GetClock:
                        Console.WriteLine($"Current System Clock: {s_bl.Admin.GetClock()}");
                        break;

                    case AdminMenuOptions.ForwardClock:
                        var unit = AskEnum<TimeUnit>("Select time unit to advance (MINUTE, HOUR, DAY, YEAR): ");
                        s_bl.Admin.ForwardClock(unit);
                        Console.WriteLine($"✅ Clock successfully advanced to: {s_bl.Admin.GetClock()}");
                        break;

                    case AdminMenuOptions.GetConfig:
                        var config = s_bl.Admin.GetConfig();
                        Console.WriteLine("--- System Configuration ---");
                        Console.WriteLine(config);
                        break;

                    case AdminMenuOptions.SetConfig:
                        var currentConfig = s_bl.Admin.GetConfig();
                        Console.WriteLine($"Current Max Delivery Range: {currentConfig.MaxDeliveryRange?.ToString() ?? "null"}. Enter new value (leave blank to keep current):");
                        var newRange = AskNullableDouble("Max Delivery Range (km): ");

                        if (newRange.HasValue || Console.ReadLine() != null)
                        {
                            currentConfig.MaxDeliveryRange = newRange;

                            s_bl.Admin.SetConfig(currentConfig);
                            Console.WriteLine("✅ Configuration successfully updated."); // I asked AI 4 the icon in order it will look cool (;
                        }
                        else
                        {
                            Console.WriteLine("Update cancelled.");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                HandleBlException(ex);
            }
        }
    }

    private static SubMenuOptions AskSubMenu(string entityName)
    {
        Console.WriteLine($"\n=== {entityName} Menu ===");
        Console.WriteLine("0) Back");
        Console.WriteLine("1) Create");
        Console.WriteLine("2) Read by ID");
        Console.WriteLine("3) Read All");
        Console.WriteLine("4) Update");
        Console.WriteLine("5) Delete by ID");
        Console.Write("Select option: ");
        return Enum.TryParse(Console.ReadLine(), out SubMenuOptions choice) ? choice : SubMenuOptions.Back;
    }

    private static void HandleCouriersMenu()
    {
        while (true)
        {
            switch (AskSubMenu("Couriers"))
            {
                case SubMenuOptions.Back: return;
                default: Console.WriteLine("\n*** Courier operations are not yet implemented in BL ***"); break;
            }
        }
    }

    private static void HandleOrdersMenu()
    {
        while (true)
        {
            switch (AskSubMenu("Orders"))
            {
                case SubMenuOptions.Back: return;
                default: Console.WriteLine("\n*** Order operations are not yet implemented in BL ***"); break;
            }
        }
    }

    private static void HandleDeliveriesMenu()
    {
        while (true)
        {
            switch (AskSubMenu("Deliveries"))
            {
                case SubMenuOptions.Back: return;
                default: Console.WriteLine("\n*** Delivery operations are not yet implemented in BL ***"); break;
            }
        }
    }

    #endregion

    static void Main(string[] args)
    {
        try
        {
            while (true)
            {
                switch (AskMainMenu())
                {
                    case MainMenuOptions.Exit: return;
                    case MainMenuOptions.Admin: HandleAdminMenu(); break;
                    case MainMenuOptions.Couriers: HandleCouriersMenu(); break;
                    case MainMenuOptions.Orders: HandleOrdersMenu(); break;
                    case MainMenuOptions.Deliveries: HandleDeliveriesMenu(); break;
                }
            }
        }
        catch (Exception ex)
        {
            HandleBlException(ex);
        }
    }
}