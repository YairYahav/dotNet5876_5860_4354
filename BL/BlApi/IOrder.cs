namespace BlApi;
using BO;
using System.Collections.Generic;
using System;

public interface IOrder
{
    int[] GetOrdersSummary(int requesterId);

    // Returns a list of orders based on the provided filters and sorting options.
    IEnumerable<BO.OpenOrderInList> GetOrders(
        int requesterId,
        BO.OrderListFilterBy? filterBy = null,
        object? filterValue = null,
        BO.OrderListOrderBy? orderBy = null);

    // Returns a list of closed deliveries for a specific courier based on the provided filters and sorting options.
    IEnumerable<BO.ClosedDeliveryInList> GerClosedDeliveries(
        int requesterId,
        int courierId,
        BO.ClosedDeliveryListFilterBy? filterBy = null,
        BO.ClosedDeliveryListOrderBy? orderBy = null);

    // Returns a list of open deliveries for a specific courier based on the provided filters and sorting options.
    IEnumerable<BO.OpenOrderInList> GetOpenDeliveries(
        int requesterId,
        int courierId,
        BO.OpenDeliveryListFilterBy? filterBy = null,
        BO.OpenDeliveryListOrderBy? orderBy = null);

    BO.Order GetOrder(int requesterId, int orderId);

    void UpdateOrder(int requesterId, BO.Order order);
    void CancelOrder(int requesterId, int orderId);

    void DeleteOrder(int requesterId, int orderId);//לשימוש רק לBL TEST

    void CreateOrder(int requesterId, BO.Order order);
    void CompleteOrderForCourier(int requesterId, int courierId, int deliveryId);
    void ChooseOrderForDelivery(int requesterId, int courierId, int orderId);

}