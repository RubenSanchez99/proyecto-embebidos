namespace Ordering.Domain.Events
{
    using Ordering.Domain.AggregatesModel.OrderAggregate;
    using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
    using EventFlow.Aggregates;
    using System.Collections.Generic;

    /// <summary>
    /// Event used when the grace period order is confirmed
    /// </summary>
    public class OrderStatusChangedToAwaitingValidationDomainEvent
         : IAggregateEvent<Order, OrderId>
    {
        public IEnumerable<OrderItem> OrderItems { get; }

        public OrderStatusChangedToAwaitingValidationDomainEvent(
            IEnumerable<OrderItem> orderItems)
        {
            OrderItems = orderItems;
        }
    }
}