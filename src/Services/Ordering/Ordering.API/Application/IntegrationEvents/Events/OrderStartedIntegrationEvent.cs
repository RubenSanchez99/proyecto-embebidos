namespace eShopOnContainers.Services.IntegrationEvents.Events
{
    using System;
    using System.Collections.Generic;
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class OrderStartedIntegrationEvent
    {
        public string UserId { get; set; }

        public string OrderId { get; set; }

        public List<OrderStockItem> OrderedItems { get; set; }
        public Guid RequestId { get; set; }
        public OrderStartedIntegrationEvent(string userId, string orderId, List<OrderStockItem> orderedItems, Guid requestId)
        {
            UserId = userId;
            OrderId = orderId;
            OrderedItems = orderedItems;
            RequestId = requestId;
        }
    }
}