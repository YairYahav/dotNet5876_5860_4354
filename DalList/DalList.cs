namespace Dal;
using DalApi;
using DO;



sealed public class DalList : IDal
{
    // Initialize the concrete implementations for each DAL interface.
    public IConfig Config { get; } = new ConfigImplementation();
    public ICourier Courier { get; } = new CourierImplementation();
    public IOrder Order { get; } = new OrderImplementation();
    public IDelivery Delivery { get; } = new DeliveryImplementation();


    // Reset all lists and configuration to initial state.
    public void ResetDB()
    {
        // We'll start by clearing the entity lists, then reset
        Courier.DeleteAll();
        Order.DeleteAll();
        Delivery.DeleteAll();

        Config.Reset();
    }
}
