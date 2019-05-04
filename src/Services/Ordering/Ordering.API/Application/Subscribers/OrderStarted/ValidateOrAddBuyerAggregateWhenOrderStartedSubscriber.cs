using System;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates;
using EventFlow.Subscribers;
using EventFlow.Core;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.BuyerAggregate.Identity;
using Ordering.Domain.Events;
using Ordering.API.Application.IntegrationEvents;
using MassTransit;
using eShopOnContainers.Services.IntegrationEvents.Events;
using Newtonsoft.Json;

namespace Ordering.API.Application.Subscribers.OrderStarted
{
    public class ValidateOrAddBuyerAggregateWhenOrderStartedSubscriber
        : ISubscribeSynchronousTo<Order, OrderId, OrderStartedDomainEvent>
    {
        private readonly IAggregateStore _aggregateStore;
        private readonly IPublishEndpoint _endpoint;
        public ValidateOrAddBuyerAggregateWhenOrderStartedSubscriber(IAggregateStore aggregateStore, IPublishEndpoint endpoint)
        {
            _aggregateStore = aggregateStore ?? throw new ArgumentNullException(nameof(aggregateStore));
            _endpoint = endpoint;
        }

        public async Task HandleAsync(IDomainEvent<Order, OrderId, OrderStartedDomainEvent> domainEvent, CancellationToken cancellationToken)
        {
            var buyerId = BuyerId.With(Guid.Parse(domainEvent.AggregateEvent.UserId));
            
            var b = await _aggregateStore
                .LoadAsync<Buyer, BuyerId>(buyerId, CancellationToken.None)
                .ConfigureAwait(false);
                        
            var cardTypeId = (domainEvent.AggregateEvent.CardTypeId != 0) ? domainEvent.AggregateEvent.CardTypeId : 1;

            await _aggregateStore.UpdateAsync<Buyer, BuyerId>(buyerId, SourceId.New,
                (buyer, c) => {
                    if (buyer.IsNew)
                    {
                        buyer.Create(domainEvent.AggregateEvent.UserId, domainEvent.AggregateEvent.UserName);
                    }

                    buyer.VerifyOrAddPaymentMethod(cardTypeId,
                                        $"Payment Method on {DateTime.UtcNow}",
                                        domainEvent.AggregateEvent.CardNumber,
                                        domainEvent.AggregateEvent.CardSecurityNumber,
                                        domainEvent.AggregateEvent.CardHolderName,
                                        domainEvent.AggregateEvent.CardExpiration,
                                        domainEvent.AggregateIdentity);
                    return Task.FromResult(0);
                }, CancellationToken.None
            ).ConfigureAwait(false); 

            var orderStatusChangedTosubmittedIntegrationEvent = new OrderStatusChangedToSubmittedIntegrationEvent(domainEvent.AggregateIdentity.Value, OrderStatus.Submitted.Name, "bob");
            await _endpoint.Publish(orderStatusChangedTosubmittedIntegrationEvent);
        }
    }
}
