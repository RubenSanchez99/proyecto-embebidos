namespace eShopOnContainers.Services.IntegrationEvents.Events
{
    public class OrderStatusChangedToStockConfirmedIntegrationEvent
    {
        public string OrderId { get; }

        public string BuyerId { get; }

        public decimal Amount { get; }

        public OrderStatusChangedToStockConfirmedIntegrationEvent(string orderId, string buyerId, decimal amount){
            this.OrderId = orderId;
            this.BuyerId = buyerId;
            this.Amount = amount;
        }
    }
}