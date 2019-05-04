using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using EventFlow.Aggregates;
using EventFlow.Subscribers;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.BuyerAggregate.Identity;
using Ordering.Domain.Events;
using Ordering.API.Application.IntegrationEvents;
using MassTransit;
using eShopOnContainers.Services.IntegrationEvents.Events;

namespace Ordering.API.Application.Subscribers.OrderPaid
{
    public class OrderStatusChangedToPaidSubscriber
        : ISubscribeSynchronousTo<Order, OrderId, OrderStatusChangedToPaidDomainEvent>
    {
        private readonly IAggregateStore _aggregateStore;
        private readonly IPublishEndpoint _endpoint;
        public OrderStatusChangedToPaidSubscriber(IAggregateStore aggregateStore, IPublishEndpoint endpoint)
        {
            _aggregateStore = aggregateStore;
            _endpoint = endpoint;
        }
        public async Task HandleAsync(IDomainEvent<Order, OrderId, OrderStatusChangedToPaidDomainEvent> domainEvent, CancellationToken cancellationToken)
        {
            var order = await _aggregateStore
                .LoadAsync<Order, OrderId>(domainEvent.AggregateIdentity, CancellationToken.None)
                .ConfigureAwait(false);

            var buyer =  await _aggregateStore
                .LoadAsync<Buyer, BuyerId>(order.GetBuyerId, CancellationToken.None)
                .ConfigureAwait(false);

            var orderStockList = domainEvent.AggregateEvent.OrderItems
                .Select(orderItem => new OrderStockItem(orderItem.ProductId, orderItem.Units));

            var orderStatusChangedToPaidIntegrationEvent = new OrderStatusChangedToPaidIntegrationEvent(
                domainEvent.AggregateIdentity.Value,
                order.OrderStatus.Name,
                "bob",
                orderStockList);

            await _endpoint.Publish(orderStatusChangedToPaidIntegrationEvent);  
        }
    }
}