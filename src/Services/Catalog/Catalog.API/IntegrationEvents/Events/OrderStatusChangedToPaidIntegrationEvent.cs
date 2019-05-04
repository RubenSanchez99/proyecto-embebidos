namespace eShopOnContainers.Services.IntegrationEvents.Events
{
    using System.Collections.Generic;
    using System.Linq;

    public class OrderStatusChangedToPaidIntegrationEvent
    {
        public string OrderId { get; set; }
        public List<OrderStockItem> OrderStockItems { get; set; }

        public OrderStatusChangedToPaidIntegrationEvent(string orderId,
            IEnumerable<OrderStockItem> orderStockItems)
        {
            OrderId = orderId;
            OrderStockItems = orderStockItems.ToList();
        }
    }
}