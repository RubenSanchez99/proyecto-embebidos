namespace eShopOnContainers.Services.IntegrationEvents.Events
{
    public class OrderStatusChangedToStockConfirmedIntegrationEvent
    {
        public string OrderId { get; }
        public string OrderStatus { get; }
        public string BuyerId { get; }
        public string BuyerName { get; }
        public decimal Amount { get; }

        public OrderStatusChangedToStockConfirmedIntegrationEvent(string orderId, string orderStatus, string buyerId, string buyerName, decimal amount)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
            BuyerId = buyerId;
            BuyerName = buyerName;
            Amount = amount;
        }
    }
}