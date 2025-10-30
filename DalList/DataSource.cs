
namespace Dal;
/// <summary>
/// In-memory data source for storing Courier, Order, and Delivery entities.
/// </summary>
internal static class DataSource
{
    internal static List<DO.Courier?> Couriers { get; } = new();// in-memory list of couriers
    internal static List<DO.Order?> Orders { get; } = new();// in-memory list of orders
    internal static List<DO.Delivery?> Deliveries { get; } = new();// in-memory list of deliveries
}
