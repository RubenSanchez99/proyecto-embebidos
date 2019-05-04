namespace eShopOnContainers.Services.IntegrationEvents.Events
{
    public class OrderStockConfirmedIntegrationEvent
    {
        public string OrderId { get; }

        public OrderStockConfirmedIntegrationEvent(string orderId) => OrderId = orderId;
    }
}