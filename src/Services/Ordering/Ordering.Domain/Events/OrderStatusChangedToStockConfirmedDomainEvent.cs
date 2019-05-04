namespace Ordering.Domain.Events
{
    using Ordering.Domain.AggregatesModel.OrderAggregate;
    using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
    using EventFlow.Aggregates;

    /// <summary>
    /// Event used when the order stock items are confirmed
    /// </summary>
    public class OrderStatusChangedToStockConfirmedDomainEvent
        : IAggregateEvent<Order, OrderId>
    {
        public OrderStatusChangedToStockConfirmedDomainEvent() {}
    }
}