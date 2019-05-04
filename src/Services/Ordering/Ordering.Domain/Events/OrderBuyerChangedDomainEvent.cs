using System;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.BuyerAggregate.Identity;
using EventFlow.Aggregates;

namespace Ordering.Domain.Events
{
    public class OrderBuyerChangedDomainEvent : IAggregateEvent<Order, OrderId>
    {
        public OrderBuyerChangedDomainEvent(BuyerId buyerId)
        {
            this.BuyerId = buyerId;
        }
        public BuyerId BuyerId { get; }
    }
}
