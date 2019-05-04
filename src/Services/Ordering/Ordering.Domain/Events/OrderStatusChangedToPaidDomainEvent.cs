namespace Ordering.Domain.Events
{
    using Ordering.Domain.AggregatesModel.OrderAggregate;
    using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
    using EventFlow.Aggregates;
    using System.Collections.Generic;

    /// <summary>
    /// Event used when the order is paid
    /// </summary>
    public class OrderStatusChangedToPaidDomainEvent
        : IAggregateEvent<Order, OrderId>
    {
        public IEnumerable<OrderItem> OrderItems { get; }

        public OrderStatusChangedToPaidDomainEvent(
            IEnumerable<OrderItem> orderItems)
        {
            OrderItems = orderItems;
        }
    }
}