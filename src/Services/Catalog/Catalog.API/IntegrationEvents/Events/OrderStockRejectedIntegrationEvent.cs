﻿namespace eShopOnContainers.Services.IntegrationEvents.Events
{
    using System.Collections.Generic;

    public class OrderStockRejectedIntegrationEvent
    {
        public string OrderId { get; }

        public List<ConfirmedOrderStockItem> OrderStockItems { get; }

        public OrderStockRejectedIntegrationEvent(string orderId,
            List<ConfirmedOrderStockItem> orderStockItems)
        {
            OrderId = orderId;
            OrderStockItems = orderStockItems;
        }
    }

    public class ConfirmedOrderStockItem
    {
        public int ProductId { get; }
        public bool HasStock { get; }

        public ConfirmedOrderStockItem(int productId, bool hasStock)
        {
            ProductId = productId;
            HasStock = hasStock;
        }
    }
}