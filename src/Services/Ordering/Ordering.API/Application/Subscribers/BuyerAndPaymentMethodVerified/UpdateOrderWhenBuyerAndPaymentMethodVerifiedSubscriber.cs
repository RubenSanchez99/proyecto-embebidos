using System;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates;
using EventFlow.Core;
using EventFlow.Subscribers;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.BuyerAggregate.Identity;
using Ordering.Domain.Events;

namespace Ordering.API.Application.Subscribers.BuyerAndPaymentMethodVerified
{
    public class UpdateOrderWhenBuyerAndPaymentMethodVerifiedSubscriber
        : ISubscribeSynchronousTo<Buyer, BuyerId, BuyerAndPaymentMethodVerifiedDomainEvent>
    {
        private readonly IAggregateStore _aggregateStore;

        public UpdateOrderWhenBuyerAndPaymentMethodVerifiedSubscriber(IAggregateStore aggregateStore)
        {
            _aggregateStore = aggregateStore ?? throw new ArgumentNullException(nameof(aggregateStore));
        }

        public async Task HandleAsync(IDomainEvent<Buyer, BuyerId, BuyerAndPaymentMethodVerifiedDomainEvent> domainEvent, CancellationToken cancellationToken)
        {
            var orderId = domainEvent.AggregateEvent.OrderId;
            
            await _aggregateStore.UpdateAsync<Order, OrderId>(orderId, SourceId.New,
                (order, c) => {
                        order.SetBuyerId(domainEvent.AggregateIdentity);
                        order.SetPaymentId(domainEvent.AggregateEvent.PaymentId);
                        return Task.FromResult(0);
                }, CancellationToken.None
            ).ConfigureAwait(false);

            return;
        }
    }
}
