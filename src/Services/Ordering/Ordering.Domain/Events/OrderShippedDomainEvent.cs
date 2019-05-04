using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
using EventFlow.Aggregates;

namespace Ordering.Domain.Events
{
    public class OrderShippedDomainEvent : IAggregateEvent<Order, OrderId>
    {

        public OrderShippedDomainEvent()
        {         
        }
    }
}
