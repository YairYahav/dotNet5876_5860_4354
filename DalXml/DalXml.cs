using DalApi;

namespace Dal;

sealed internal class DalXml : IDal
{
    private DalXml() { }

    public static IDal Instance { get; } = new DalXml();


    public IConfig Config { get; } = new ConfigImplementation();// Initialize the concrete implementation for configuration management.

    public ICourier Courier { get; } = new CourierImplementation();// Initialize the concrete implementation for courier management.

    public IOrder Order { get; } = new OrderImplementation();// Initialize the concrete implementation for order management.

    public IDelivery Delivery { get; } = new DeliveryImplementation();// Initialize the concrete implementation for delivery management.

    /// <summary>
    /// Resets the database to its initial state by clearing all data and restoring default configurations.
    /// </summary>
    /// <remarks>This method deletes all records from the Courier, Order, and Delivery entities, and resets
    /// the application configuration to its default values.  Use this method with caution as it results in irreversible
    /// data loss.</remarks>
    public void ResetDB()
    {

        Courier.DeleteAll();
        Order.DeleteAll();
        Delivery.DeleteAll();
        Config.Reset();
    }
}
