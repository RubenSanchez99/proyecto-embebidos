namespace eShopOnContainers.Services.IntegrationEvents.Events
{
    public class OrderStatusChangedToSubmittedIntegrationEvent
    {
        public string OrderId { get; }
        public string OrderStatus { get; }
        public string BuyerName { get; }

        public OrderStatusChangedToSubmittedIntegrationEvent(string orderId, string orderStatus, string buyerName)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
            BuyerName = buyerName;
        }
    }
}
