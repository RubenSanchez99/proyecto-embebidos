using MassTransit;
using System.Collections.Generic;

namespace eShopOnContainers.Services.IntegrationEvents.Events
{
    public class OrderStatusChangedToAwaitingValidationIntegrationEvent
    {
        public string OrderId { get; }
        public string OrderStatus { get; }
        public string BuyerName { get; }

        public OrderStatusChangedToAwaitingValidationIntegrationEvent(string orderId, string orderStatus, string buyerName)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
            BuyerName = buyerName;
        }
    }
}
