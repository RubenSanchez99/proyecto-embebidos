namespace eShopOnContainers.Services.IntegrationEvents.Events
{
    public class GracePeriodConfirmedIntegrationEvent
    {
        public string OrderId { get; }

        public GracePeriodConfirmedIntegrationEvent(string orderId) =>
            OrderId = orderId;
    }
}
