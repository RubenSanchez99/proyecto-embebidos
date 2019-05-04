using System;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
using Ordering.Domain.AggregatesModel.BuyerAggregate.Identity;
using EventFlow.Aggregates;

namespace Ordering.Domain.Events
{
    public class OrderPaymentMethodChangedDomainEvent : IAggregateEvent<Order, OrderId>
    {
        public OrderPaymentMethodChangedDomainEvent(PaymentMethodId paymentMethodId)
        {
            this.PaymentMethodId = paymentMethodId;

        }
        public PaymentMethodId PaymentMethodId { get; }
    }
}
