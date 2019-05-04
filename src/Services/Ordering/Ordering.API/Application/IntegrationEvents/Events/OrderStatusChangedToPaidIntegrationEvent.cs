namespace eShopOnContainers.Services.IntegrationEvents.Events
{
    using System.Collections.Generic;

    public class OrderStatusChangedToPaidIntegrationEvent
    {
        public string OrderId { get; }
        public string OrderStatus { get; }
        public string BuyerName { get; }
        public IEnumerable<OrderStockItem> OrderStockItems { get; }

        public OrderStatusChangedToPaidIntegrationEvent(string orderId,
            string orderStatus,
            string buyerName,
            IEnumerable<OrderStockItem> orderStockItems)
        {
            OrderId = orderId;
            OrderStockItems = orderStockItems;
            OrderStatus = orderStatus;
            BuyerName = buyerName;
        }
    }
}
