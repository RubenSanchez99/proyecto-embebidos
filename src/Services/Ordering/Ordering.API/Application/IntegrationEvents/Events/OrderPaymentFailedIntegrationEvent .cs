namespace eShopOnContainers.Services.IntegrationEvents.Events
{
    public class OrderPaymentFailedIntegrationEvent 
    {
        public string OrderId { get; }

        public OrderPaymentFailedIntegrationEvent(string orderId) => OrderId = orderId;
    }
}