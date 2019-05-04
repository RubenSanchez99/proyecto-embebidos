namespace eShopOnContainers.Services.IntegrationEvents.Events
{
    public class OrderStockSentForOrderIntegrationEvent
    {
        public OrderStockSentForOrderIntegrationEvent(string orderId)
        {
            this.OrderId = orderId;

        }
        public string OrderId { get; set; }
    }
}