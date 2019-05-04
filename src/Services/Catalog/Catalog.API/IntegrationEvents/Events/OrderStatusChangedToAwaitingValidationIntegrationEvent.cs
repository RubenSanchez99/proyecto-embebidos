﻿namespace eShopOnContainers.Services.IntegrationEvents.Events
{
    using System.Collections.Generic;
    using System.Linq;

    public class OrderStatusChangedToAwaitingValidationIntegrationEvent
    {
        public string OrderId { get; set; }
        public List<OrderStockItem> OrderStockItems { get; set; }

        public OrderStatusChangedToAwaitingValidationIntegrationEvent(string orderId,
            IEnumerable<OrderStockItem> orderStockItems)
        {
            OrderId = orderId;
            OrderStockItems = orderStockItems.ToList();
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