using System;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
using EventFlow.Aggregates;

namespace Ordering.Domain.Events
{
    public class OrderUnitsAdded : IAggregateEvent<Order, OrderId>
    {
        public OrderUnitsAdded(int unitsAdded)
        {
            this.UnitsAdded = unitsAdded;

        }
        public int UnitsAdded { get; }

    }
}
