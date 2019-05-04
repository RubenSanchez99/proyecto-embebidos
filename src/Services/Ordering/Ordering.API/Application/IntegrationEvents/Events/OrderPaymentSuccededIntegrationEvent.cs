namespace eShopOnContainers.Services.IntegrationEvents.Events
{
    public class OrderPaymentSuccededIntegrationEvent
    {
        public string OrderId { get; }

        public OrderPaymentSuccededIntegrationEvent(string orderId) => OrderId = orderId;
    }
}