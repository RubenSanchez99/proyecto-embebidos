using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
using EventFlow.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.Events
{
    public class OrderCancelledDomainEvent : IAggregateEvent<Order, OrderId>
    {
        public string Description { get; }

        public OrderCancelledDomainEvent(string description)
        {
            Description = description;
        }
    }
}
