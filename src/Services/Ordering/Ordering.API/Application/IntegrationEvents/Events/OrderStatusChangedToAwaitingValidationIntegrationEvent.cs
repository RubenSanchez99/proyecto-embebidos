namespace eShopOnContainers.Services.IntegrationEvents.Events
{
    using System.Collections.Generic;
    using System.Linq;

    public class OrderStatusChangedToAwaitingValidationIntegrationEvent
    {
        public string OrderId { get;}
        public string OrderStatus { get; }
        public List<OrderStockItem> OrderStockItems { get; }
        public string BuyerName { get; }

        public OrderStatusChangedToAwaitingValidationIntegrationEvent(string orderId, string orderStatus,
            IEnumerable<OrderStockItem> orderStockItems, string buyerName)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
            OrderStockItems = orderStockItems.ToList();
            BuyerName = buyerName;
        }
    }

    public class OrderStockItem
    {
        public int ProductId { get; }
        public int Units { get; }

        public OrderStockItem(int productId, int units)
        {
            ProductId = productId;
            Units = units;
        }
    }
}