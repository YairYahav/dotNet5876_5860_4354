using Dal;
using DalApi;
using DO;

namespace DalTest
{
    internal class Program
    {
        // Again, the old stge (stage 1):
        //private static ICourier? s_dalCourier = new CourierImplementation();
        //private static IDelivery? s_dalDelivery = new DeliveryImplementation();
        //private static IOrder? s_dalOrder = new OrderImplementation();
        //private static IConfig? s_dalConfig = new ConfigImplementation();

        //static readonly IDal s_dal = new DalList(); // Stage 2
        //static readonly IDal s_dal = new DalXml();  // Stage 3
        static readonly IDal s_dal = Factory.Get;

        /// <summary>
        /// Main menu options enumeration.
        /// </summary>
        private enum MainMenuOptions
        {
            Exit,
            Couriers,
            Orders,
            Deliveries,
            InitDatabase,
            ShowAll,
            ConfigMenu,
            ResetAll
        }

        /// <summary>
        /// Sub-menu options enumeration.
        /// </summary>
        private enum subMenuOptions
        {
            Back,
            Create,
            Read,
            ReadAll,
            Update,
            Delete,
            DeleteAll
        }

        /// <summary>
        /// Configuration sub-menu options enumeration.
        /// </summary>
        private enum ConfigSubMenu
        {
            Back,
            TickMinute,
            TickHour,
            ShowClock,
            SetMaxRange,
            ShowMaxRange,
            ResetConfig
        }

        /// <summary>
        /// Displays the main menu options to the user and prompts for a selection.
        /// </summary>
        /// <remarks>The method presents a list of options corresponding to the main menu and reads the
        /// user's input. If the input matches a valid <see cref="MainMenuOptions"/> value, the corresponding option is
        /// returned. If the input is invalid or cannot be parsed, the method defaults to returning <see
        /// cref="MainMenuOptions.Exit"/>.</remarks>
        /// <returns>A <see cref="MainMenuOptions"/> value representing the user's selection. Defaults to <see
        /// cref="MainMenuOptions.Exit"/> if the input is invalid.</returns>
        private static MainMenuOptions AskMainMenu()
        {
            Console.WriteLine("\n=== Main Menu ===");
            Console.WriteLine("0) Exit");
            Console.WriteLine("1) Couriers");
            Console.WriteLine("2) Orders");
            Console.WriteLine("3) Deliveries");
            Console.WriteLine("4) Initialize database");
            Console.WriteLine("5) Show all data");
            Console.WriteLine("6) Configuration");
            Console.WriteLine("7) Reset all (lists + config)");
            Console.Write("Choose: ");
            return Enum.TryParse(Console.ReadLine(), out MainMenuOptions choice) ? choice : MainMenuOptions.Exit;
        }

        /// <summary>
        /// Displays a submenu for the specified entity and prompts the user to select an option.
        /// </summary>
        /// <remarks>The submenu includes options for common CRUD operations and additional actions such
        /// as "DeleteAll". The user is prompted to enter a choice, which is parsed into a <see cref="subMenuOptions"/>
        /// enumeration value.</remarks>
        /// <param name="entityName">The name of the entity for which the submenu is displayed. This is used to customize the menu title.</param>
        /// <returns>A <see cref="subMenuOptions"/> value representing the user's choice. If the input is invalid, the method
        /// returns <see cref="subMenuOptions.Back"/>.</returns>
        private static subMenuOptions AskSubMenu(string entityName)
        {
            Console.WriteLine($"\n=== {entityName} Menu ===");
            Console.WriteLine("0) Back");
            Console.WriteLine("1) Create");
            Console.WriteLine("2) Read");
            Console.WriteLine("3) ReadAll");
            Console.WriteLine("4) Update");
            Console.WriteLine("5) Delete");
            Console.WriteLine("6) DeleteAll");
            Console.Write("Choose: ");
            return Enum.TryParse(Console.ReadLine(), out subMenuOptions choice) ? choice : subMenuOptions.Back;
        }

        /// <summary>
        /// Handles the "Couriers" submenu, providing options to create, read, update, delete, or list couriers.
        /// </summary>
        /// <remarks>This method presents a looped menu for managing couriers, allowing the user to
        /// perform CRUD operations  and navigate back to the previous menu. Each menu option corresponds to a specific
        /// operation, such as  creating a new courier, reading courier details, updating existing couriers, or deleting
        /// couriers. Exceptions encountered during operations are caught and their messages are displayed to the
        /// console.</remarks>
        private static void HandleCouriersMenu()
        {
            while (true)
            {
                switch (AskSubMenu("Couriers"))
                {
                    case subMenuOptions.Back: return;
                    case subMenuOptions.Create:
                        try
                        {
                            var c = ReadCourierFromConsoleForCreate();
                            s_dal.Courier!.Create(c);
                            Console.WriteLine("Created.");
                        }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }
                        break;

                    case subMenuOptions.Read:
                        try
                        {
                            int id = AskInt("Courier Id: ");
                            var c = s_dal.Courier!.Read(id);
                            Console.WriteLine(c is null ? "Not found." : c);
                        }
                        catch (Exception ex) 
                        { Console.WriteLine(ex.Message); }
                        break;

                    case subMenuOptions.ReadAll:
                        try
                        {
                            foreach (var c in s_dal.Courier!.ReadAll())
                                Console.WriteLine(c);
                        }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }
                        break;

                    case subMenuOptions.Update:
                        try
                        {
                            int id = AskInt("Courier Id to update: ");
                            var existing = s_dal.Courier!.Read(id);
                            Console.WriteLine(existing is null ? "Not found." : existing);
                            if (existing is not null)
                            {
                                var updated = ReadCourierFromConsoleForUpdate(existing);
                                s_dal.Courier!.Update(updated);
                                Console.WriteLine("Updated.");
                            }
                        }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }
                        break;

                    case subMenuOptions.Delete:
                        try
                        {
                            int id = AskInt("Courier Id to delete: ");
                            s_dal.Courier!.Delete(id);
                            Console.WriteLine("Deleted.");
                        }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }
                        break;

                    case subMenuOptions.DeleteAll:
                        try { s_dal.Courier!.DeleteAll(); Console.WriteLine("All couriers deleted."); }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }
                        break;
                }
            }
        }

        /// <summary>
        /// Handles the "Orders" submenu, providing options to create, read, update, and delete orders.
        /// </summary>
        /// <remarks>This method displays a submenu for managing orders and processes user input to
        /// perform the selected operation. Available operations include creating a new order, reading an order by ID,
        /// reading all orders, updating an existing order, deleting an order by ID, and deleting all orders. The method
        /// runs in a loop until the user selects the "Back" option.</remarks>
        private static void HandleOrdersMenu()
        {
            while (true)
            {
                switch (AskSubMenu("Orders"))
                {
                    case subMenuOptions.Back: return;
                    case subMenuOptions.Create:
                        try
                        {
                            var o = ReadOrderFromConsoleForCreate();
                            s_dal.Order!.Create(o);
                            Console.WriteLine("Created.");
                        }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }
                        break;

                    case subMenuOptions.Read:
                        try
                        {
                            int id = AskInt("Order Id: ");
                            var o = s_dal.Order!.Read(id);
                            Console.WriteLine(o is null ? "Not found." : o);
                        }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }
                        break;

                    case subMenuOptions.ReadAll:
                        try
                        {
                            foreach (var o in s_dal.Order!.ReadAll())
                                Console.WriteLine(o);
                        }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }
                        break;

                    case subMenuOptions.Update:
                        try
                        {
                            int id = AskInt("Order Id to update: ");
                            var existing = s_dal.Order!.Read(id);
                            Console.WriteLine(existing is null ? "Not found." : existing);
                            if (existing is not null)
                            {
                                var updated = ReadOrderFromConsoleForUpdate(existing);
                                s_dal.Order!.Update(updated);
                                Console.WriteLine("Updated.");
                            }
                        }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }
                        break;

                    case subMenuOptions.Delete:
                        try
                        {
                            int id = AskInt("Order Id to delete: ");
                            s_dal.Order!.Delete(id);
                            Console.WriteLine("Deleted.");
                        }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }
                        break;

                    case subMenuOptions.DeleteAll:
                        try { s_dal.Order!.DeleteAll(); Console.WriteLine("All orders deleted."); }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }
                        break;
                }
            }
        }

        /// <summary>
        /// Handles the "Deliveries" menu, providing options to create, read, update, and delete delivery records.
        /// </summary>
        /// <remarks>This method presents a submenu for managing deliveries and performs the selected
        /// operation based on user input. The available options include creating a new delivery, reading a specific
        /// delivery by ID, reading all deliveries, updating an existing delivery, deleting a specific delivery by ID,
        /// and deleting all deliveries. The method runs in a loop until the user selects the "Back" option to exit the
        /// submenu.</remarks>
        private static void HandleDeliveriesMenu()
        {
            while (true)
            {
                switch (AskSubMenu("Deliveries"))
                {
                    case subMenuOptions.Back: return;
                    case subMenuOptions.Create:
                        try
                        {
                            var d = ReadDeliveryFromConsoleForCreate();
                            s_dal.Delivery!.Create(d);
                            Console.WriteLine("Created.");
                        }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }
                        break;

                    case subMenuOptions.Read:
                        try
                        {
                            int id = AskInt("Delivery Id: ");
                            var d = s_dal.Delivery!.Read(id);
                            Console.WriteLine(d is null ? "Not found." : d);
                        }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }
                        break;

                    case subMenuOptions.ReadAll:
                        try
                        {
                            foreach (var d in s_dal.Delivery!.ReadAll())
                                Console.WriteLine(d);
                        }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }
                        break;

                    case subMenuOptions.Update:
                        try
                        {
                            int id = AskInt("Delivery Id to update: ");
                            var existing = s_dal.Delivery!.Read(id);
                            Console.WriteLine(existing is null ? "Not found." : existing);
                            if (existing is not null)
                            {
                                var updated = ReadDeliveryFromConsoleForUpdate(existing);
                                s_dal.Delivery!.Update(updated);
                                Console.WriteLine("Updated.");
                            }
                        }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }
                        break;

                    case subMenuOptions.Delete:
                        try
                        {
                            int id = AskInt("Delivery Id to delete: ");
                            s_dal.Delivery!.Delete(id);
                            Console.WriteLine("Deleted.");
                        }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }
                        break;

                    case subMenuOptions.DeleteAll:
                        try { s_dal.Delivery!.DeleteAll(); Console.WriteLine("All deliveries deleted."); }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }
                        break;
                }
            }
        }

        /// <summary>
        /// Prompts the user with the specified message and retrieves the input as a string.
        /// </summary>
        /// <param name="prompt">The message displayed to the user before reading input.</param>
        /// <returns>The string entered by the user. If the input is null, an empty string is returned.</returns>
        private static string AskString(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? "";
        }

        /// <summary>
        /// Reads a nullable double value from the console after displaying a prompt.
        /// </summary>
        /// <remarks>The method expects the user to input a valid numeric value. If the input is invalid
        /// or cannot be parsed as a double, an exception may be thrown.</remarks>
        /// <param name="prompt">The message displayed to the user before reading input.</param>
        /// <returns>A nullable double value parsed from the user's input, or <see langword="null"/> if the input is empty or
        /// consists only of whitespace.</returns>
        private static double? ReadNullableDouble(string prompt)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            return string.IsNullOrWhiteSpace(s) ? null : double.Parse(s);
        }

        /// <summary>
        /// Reads a string input from the console and returns it as a nullable string.
        /// </summary>
        /// <param name="prompt">The message displayed to the user before reading input.</param>
        /// <returns>The input string entered by the user, or <see langword="null"/> if the input is empty or consists only of
        /// whitespace.</returns>
        private static string? ReadNullableString(string prompt)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            return string.IsNullOrWhiteSpace(s) ? null : s;
        }


        /// <summary>
        /// Prompts the user to enter an integer and repeatedly requests input until a valid integer is provided.
        /// </summary>
        /// <param name="prompt">The message displayed to the user to indicate what input is expected.</param>
        /// <returns>The integer value entered by the user.</returns>
        private static int AskInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (int.TryParse(s, out var v)) return v;
                Console.WriteLine("Please enter a valid integer.");
            }
        }

        /// <summary>
        /// Prompts the user to enter a double-precision floating-point number and validates the input.
        /// </summary>
        /// <remarks>The method repeatedly prompts the user until a valid number is entered. If the input
        /// is invalid, an error message is displayed, and the user is prompted again.</remarks>
        /// <param name="prompt">The message displayed to the user to indicate what input is expected.</param>
        /// <returns>The double-precision floating-point number entered by the user.</returns>
        private static double AskDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (double.TryParse(s, out var v)) return v;
                Console.WriteLine("Please enter a valid number.");
            }
        }

        /// <summary>
        /// Prompts the user with a message and reads a boolean value from the console.
        /// </summary>
        /// <remarks>The input is case-insensitive and must be parsable as a boolean value. If the input
        /// is invalid, the user is prompted again until a valid response is provided.</remarks>
        /// <param name="prompt">The message displayed to the user, indicating the expected input.</param>
        /// <returns><see langword="true"/> if the user enters "true"; <see langword="false"/> if the user enters "false". The
        /// method continues prompting until a valid boolean value is entered.</returns>
        private static bool AskBool(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (bool.TryParse(s, out var v)) return v;
                Console.WriteLine("Please enter true or false.");
            }
        }

        /// <summary>
        /// Prompts the user for a double-precision floating-point number, allowing for null input.
        /// </summary>
        /// <remarks>If the input is invalid, a message is displayed to the user indicating that the value
        /// will remain null.</remarks>
        /// <param name="prompt">The message displayed to the user to request input.</param>
        /// <returns>The parsed double value if the user provides valid input; otherwise, <see langword="null"/> if the input is
        /// empty, whitespace, or invalid.</returns>
        private static double? AskNullableDouble(string prompt)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (double.TryParse(s, out var v)) return v;
            Console.WriteLine("Invalid number. Keeping null.");
            return null;
        }

        /// <summary>
        /// Prompts the user with the specified message and reads a date and time input from the console.
        /// </summary>
        /// <remarks>The input must be in a valid date and time format recognized by the current culture. 
        /// An exception will be thrown if the input is invalid or null.</remarks>
        /// <param name="prompt">The message displayed to the user to prompt for input.</param>
        /// <returns>A <see cref="DateTime"/> value representing the date and time entered by the user.</returns>
        private static DateTime AskDateTime(string prompt)
        {
            Console.Write(prompt);
            return DateTime.Parse(Console.ReadLine()!);
        }


        /// <summary>
        /// Reads courier details from the console input and creates a new <see cref="Courier"/> instance.
        /// </summary>
        /// <remarks>This method prompts the user to input various details about a courier, including
        /// their ID, full name, contact information, employment status, and delivery preferences. The input is
        /// validated based on the expected data types, and the method returns a fully initialized <see cref="Courier"/>
        /// object.</remarks>
        /// <returns>A <see cref="Courier"/> instance populated with the details provided by the user.</returns>
        private static Courier ReadCourierFromConsoleForCreate()
        {
            int id = AskInt("Id: ");
            string fullName = AskString("Full name: ");
            string phone = AskString("Phone: ");
            string email = AskString("Email: ");
            string password = AskString("Password: ");
            bool isActive = AskBool("IsActive (true/false): ");
            double? personalMax = ReadNullableDouble("Personal max range km (empty for no limit): ");
            var kind = (DeliveryType)AskInt("ShippingKind (0 Car, 1 Motorcycle, 2 Bicycle, 3 Walking): ");
            DateTime employmentStartDate = AskDateTime("Start work date-time (yyyy-MM-dd HH:mm): ");

            return new Courier(
                Id: id,
                FullName: fullName,
                PhoneNumber: phone,
                Gmail: email,
                Password: password,
                IsActive: isActive,
                MaxPersonalDeliveryDistance: personalMax,
                DeliveryType: kind,
                EmploymentStartDate: employmentStartDate
            );
        }

        
        /// <summary>
        /// Reads updated courier information from the console, allowing the user to modify specific fields of an
        /// existing courier.
        /// </summary>
        /// <remarks>This method prompts the user to input new values for each field of the courier. If
        /// the user provides an empty input for a field, the original value from the <paramref name="existing"/> object
        /// is retained. Boolean and numeric fields are parsed from the user input, and invalid inputs may result in
        /// exceptions.</remarks>
        /// <param name="existing">The existing <see cref="Courier"/> object whose details are to be updated.</param>
        /// <returns>A new <see cref="Courier"/> object containing the updated information. Fields not modified by the user
        /// retain their original values.</returns>
        private static Courier ReadCourierFromConsoleForUpdate(Courier existing)
        {
            string fullName = AskString($"Full name ({existing.FullName}): ");
            string phone = AskString($"Phone ({existing.PhoneNumber}): ");
            string email = AskString($"Gmail ({existing.Gmail}): ");
            string password = AskString("Password (keep empty to not change): ");
            string activeStr = AskString($"IsActive ({existing.IsActive}): ");
            double? personalMax = ReadNullableDouble($"Personal max range km ({existing.MaxPersonalDeliveryDistance?.ToString() ?? "null"}): ");
            string kindStr = AskString($"DeliveryType ({(int)existing.DeliveryType}): ");

            return existing with
            {
                FullName = string.IsNullOrWhiteSpace(fullName) ? existing.FullName : fullName,
                PhoneNumber = string.IsNullOrWhiteSpace(phone) ? existing.PhoneNumber : phone,
                Gmail = string.IsNullOrWhiteSpace(email) ? existing.Gmail : email,
                Password = string.IsNullOrWhiteSpace(password) ? existing.Password : password,
                IsActive = string.IsNullOrWhiteSpace(activeStr) ? existing.IsActive : bool.Parse(activeStr),
                MaxPersonalDeliveryDistance = double.IsNaN(personalMax ?? double.NaN) ? existing.MaxPersonalDeliveryDistance : personalMax,
                DeliveryType = string.IsNullOrWhiteSpace(kindStr) ? existing.DeliveryType : (DeliveryType)int.Parse(kindStr)
            };
        }

        /// <summary>
        /// Reads order details from the console and creates a new <see cref="Order"/> instance.
        /// </summary>
        /// <remarks>This method prompts the user to input various details about an order, including its
        /// type,  address, customer information, and other optional attributes. The input is validated and  converted
        /// to the appropriate types before constructing the <see cref="Order"/> object.</remarks>
        /// <returns>A new <see cref="Order"/> instance populated with the details provided by the user.</returns>
        private static Order ReadOrderFromConsoleForCreate()
        {
            var type = (OrderType)AskInt("TypeOfOrder (int): ");
            string addr = AskString("Address: ");
            double lat = AskDouble("Latitude: ");
            double lon = AskDouble("Longitude: ");
            string cust = AskString("Customer name: ");
            string phone = AskString("Customer phone: ");
            bool isFrag = AskBool("Is fragile (true/false): ");
            double? vol = ReadNullableDouble("Volume (empty for null): ");
            double? weight = ReadNullableDouble("Weight (empty for null): ");
            string? desc = ReadNullableString("Description (empty for null): ");
            DateTime opened = AskDateTime("OpenedAt (yyyy-MM-dd HH:mm): ");

            return new Order(
                Id: 0,
                OrderType: type,
                AddressOfOrder: addr,
                Latitude: lat,
                Longitude: lon,
                CustomerName: cust,
                CustomerPhone: phone,
                IsFrag: isFrag,
                Volume: vol,
                Weight: weight,
                DescriptionOfOrder: desc,
                OrderPlacementTime: opened
            );
        }


        /// <summary>
        /// Reads updated order details from the console, allowing the user to modify the address and description of an
        /// existing order.
        /// </summary>
        /// <param name="existing">The existing <see cref="Order"/> to be updated. This provides default values for fields not modified by the
        /// user.</param>
        /// <returns>A new <see cref="Order"/> instance with updated values for the address and description, or the original
        /// values if no updates were provided.</returns>
        private static Order ReadOrderFromConsoleForUpdate(Order existing)
        {
            string addr = ReadNullableString($"Address ({existing.AddressOfOrder}): ");
            string desc = ReadNullableString($"Description ({existing.DescriptionOfOrder ?? "null"}): ");
            OrderType type = existing.OrderType;


            return existing with
            {
                AddressOfOrder = string.IsNullOrWhiteSpace(addr) ? existing.AddressOfOrder : addr,
                DescriptionOfOrder = string.IsNullOrWhiteSpace(desc) ? existing.DescriptionOfOrder : desc
            };
        }


        /// <summary>
        /// Reads delivery details from the console to create a new <see cref="Delivery"/> instance.
        /// </summary>
        /// <remarks>This method prompts the user to input various details required to create a new
        /// delivery, including order ID, courier ID, delivery type, start time, and optional end time details. The
        /// method validates and parses the input, returning a fully constructed <see cref="Delivery"/>
        /// object.</remarks>
        /// <returns>A new <see cref="Delivery"/> instance populated with the user-provided input values.</returns>
        private static Delivery ReadDeliveryFromConsoleForCreate()
        {
            int orderId = AskInt("OrderId: ");
            int courierId = AskInt("CourierId: ");
            var kind = (DeliveryType)AskInt("ShippingKind (int): ");
            DateTime start = AskDateTime("StartTime (yyyy-MM-dd HH:mm): ");
            string endKindStr = ReadNullableString("EndKind (int, empty for null): ");
            string endTimeStr = ReadNullableString("EndTime (yyyy-MM-dd HH:mm, empty for null): ");

            TypeOfDeliveryCompletionTime? ek = string.IsNullOrWhiteSpace(endKindStr) ? null : (TypeOfDeliveryCompletionTime)int.Parse(endKindStr);
            DateTime? et = string.IsNullOrWhiteSpace(endTimeStr) ? null : DateTime.Parse(endTimeStr);

            return new Delivery(
                Id: 0,
                OrderId: orderId,
                CourierId: courierId,
                DeliveryType: kind,
                DeliveryStartTime: start,
                ActualDistance: null,
                TypeOfDeliveryCompletionTime: ek,
                DeliveryCompletionTime: et
            );
        }


        /// <summary>
        /// Reads updated delivery details from the console and returns a modified <see cref="Delivery"/> object based
        /// on the provided existing delivery.
        /// </summary>
        /// <remarks>This method prompts the user to input new values for the delivery's completion type
        /// and time. If the user provides no input for a field, the existing value is retained. Input is validated to
        /// ensure it conforms to the expected formats.</remarks>
        /// <param name="existing">The existing <see cref="Delivery"/> object to update. This object provides default values for fields not
        /// updated by the user.</param>
        /// <returns>A new <see cref="Delivery"/> object with updated values for <see
        /// cref="Delivery.TypeOfDeliveryCompletionTime"/> and <see cref="Delivery.DeliveryCompletionTime"/>, or the
        /// original values if no updates are provided.</returns>
        private static Delivery ReadDeliveryFromConsoleForUpdate(Delivery existing)
        {
            string endKindStr = ReadNullableString($"EndKind ({existing.TypeOfDeliveryCompletionTime?.ToString() ?? "null"}): ");
            string endTimeStr = ReadNullableString($"EndTime ({existing.DeliveryCompletionTime?.ToString("yyyy-MM-dd HH:mm") ?? "null"}): ");

            return existing with
            {
                TypeOfDeliveryCompletionTime = string.IsNullOrWhiteSpace(endKindStr) ? existing.TypeOfDeliveryCompletionTime : (TypeOfDeliveryCompletionTime)int.Parse(endKindStr),
                DeliveryCompletionTime = string.IsNullOrWhiteSpace(endTimeStr) ? existing.DeliveryCompletionTime : DateTime.Parse(endTimeStr)
            };
        }
        private static void HandleConfigMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== Configuration Menu ===");
                Console.WriteLine("0) Back");
                Console.WriteLine("1) Tick clock by 1 minute");
                Console.WriteLine("2) Tick clock by 1 hour");
                Console.WriteLine("3) Show current clock");
                Console.WriteLine("4) Set MaxDeliveryRange");
                Console.WriteLine("5) Show MaxDeliveryRange");
                Console.WriteLine("6) Reset configuration");
                Console.Write("Choose: ");

                if (!Enum.TryParse(Console.ReadLine(), out ConfigSubMenu choice))
                    choice = ConfigSubMenu.Back;

                try
                {
                    switch (choice)
                    {
                        case ConfigSubMenu.Back: return;
                        case ConfigSubMenu.TickMinute:
                            s_dal.Config!.Clock = s_dal.Config.Clock.AddMinutes(1);
                            Console.WriteLine($"Clock: {s_dal.Config.Clock}");
                            break;
                        case ConfigSubMenu.TickHour:
                            s_dal.Config!.Clock = s_dal.Config.Clock.AddHours(1);
                            Console.WriteLine($"Clock: {s_dal.Config.Clock}");
                            break;
                        case ConfigSubMenu.ShowClock:
                            Console.WriteLine(s_dal.Config!.Clock);
                            break;
                        case ConfigSubMenu.SetMaxRange:
                            s_dal.Config!.MaxDeliveryRange = AskNullableDouble("Max delivery range (km, empty = null): ");
                            Console.WriteLine("Set.");
                            break;
                        case ConfigSubMenu.ShowMaxRange:
                            Console.WriteLine($"MaxDeliveryRange: {s_dal.Config!.MaxDeliveryRange?.ToString() ?? "null"} km");
                            break;
                        case ConfigSubMenu.ResetConfig:
                            s_dal.Config!.Reset();
                            Console.WriteLine("Configuration reset.");
                            break;
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
        }
        /// <summary>
        /// Displays all data for couriers, orders, and deliveries in the console.
        /// </summary>
        /// <remarks>This method retrieves and prints all records from the data access layers for
        /// couriers, orders, and deliveries. If an exception occurs during data retrieval or printing, the exception
        /// message is displayed in the console.</remarks>
        private static void PrintAllData()
        {
            try
            {
                Console.WriteLine("\n-- Couriers --");
                foreach (var c in s_dal.Courier!.ReadAll()) Console.WriteLine(c);

                Console.WriteLine("\n-- Orders --");
                foreach (var o in s_dal.Order!.ReadAll()) Console.WriteLine(o);

                Console.WriteLine("\n-- Deliveries --");
                foreach (var d in s_dal.Delivery!.ReadAll()) Console.WriteLine(d);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        /// <summary>
        /// The entry point of the application. Provides a main menu for interacting with the system, allowing users to
        /// manage couriers, orders, deliveries, and configurations, as well as initialize or reset the database.
        /// </summary>
        /// <remarks>This method runs an infinite loop to display the main menu and handle user input
        /// until the user chooses to exit. It provides options for managing various aspects of the system, including
        /// initializing the database, clearing all data, and accessing submenus for specific operations. Exceptions are
        /// caught and logged to the console to ensure the application does not crash unexpectedly.</remarks>
        /// <param name="args">An array of command-line arguments passed to the application. Currently unused.</param>
        static void Main(string[] args)
        {
            try
            {
                while (true)
                {
                    switch (AskMainMenu())
                    {
                        case MainMenuOptions.Exit:
                            return;

                        case MainMenuOptions.Couriers:
                            HandleCouriersMenu();
                            break;

                        case MainMenuOptions.Orders:
                            HandleOrdersMenu();
                            break;

                        case MainMenuOptions.Deliveries:
                            HandleDeliveriesMenu();
                            break;

                        case MainMenuOptions.InitDatabase:
                            // Again, Stage 1:
                            //Initialization.Do(s_dalCourier, s_dalDelivery, s_dalOrder, s_dalConfig); 

                            // Stage 2:
                            Initialization.Do(); // removing the parameter "s_dal" in Stage 4.
                            Console.WriteLine("Database initialized.");
                            break;

                        case MainMenuOptions.ShowAll:
                            PrintAllData();
                            break;

                        case MainMenuOptions.ConfigMenu:
                            HandleConfigMenu();
                            break;

                        case MainMenuOptions.ResetAll:
                            s_dal.Courier!.DeleteAll();
                            s_dal.Order!.DeleteAll();
                            s_dal.Delivery!.DeleteAll();
                            s_dal.Config!.Reset();
                            Console.WriteLine("All data cleared and configuration reset.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled error: {ex.Message}");
            }
        }

    }
}
