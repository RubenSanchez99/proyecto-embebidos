using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.BuyerAggregate.Identity;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
using EventFlow.Aggregates;

namespace Ordering.Domain.Events
{
    public class BuyerAndPaymentMethodVerifiedDomainEvent
        : IAggregateEvent<Buyer, BuyerId>
    {
        public PaymentMethodId PaymentId { get; private set; }
        public OrderId OrderId { get; private set; }

        public BuyerAndPaymentMethodVerifiedDomainEvent(PaymentMethodId paymentId, OrderId orderId)
        {
            PaymentId = paymentId;
            OrderId = orderId;
        }
    }
}
