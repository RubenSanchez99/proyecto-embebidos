using System;
using System.Threading;
using System.Threading.Tasks;
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
using System.Linq;
using Newtonsoft.Json;

namespace Ordering.API.Application.Subscribers.OrderStarted
{
    public class OrderStatusChangedToStockConfirmedSubscriber
        : ISubscribeSynchronousTo<Order, OrderId, OrderStatusChangedToStockConfirmedDomainEvent>
    {
        private readonly IAggregateStore _aggregateStore;
        private readonly IPublishEndpoint _endpoint;

        public OrderStatusChangedToStockConfirmedSubscriber(IAggregateStore aggregateStore, IPublishEndpoint endpoint)
        {
            _aggregateStore = aggregateStore;
            _endpoint = endpoint;
        }

        public async Task HandleAsync(IDomainEvent<Order, OrderId, OrderStatusChangedToStockConfirmedDomainEvent> domainEvent, CancellationToken cancellationToken)
        {
            var order = await _aggregateStore
                .LoadAsync<Order, OrderId>(domainEvent.AggregateIdentity, CancellationToken.None)
                .ConfigureAwait(false);

            var buyer =  await _aggregateStore
                .LoadAsync<Buyer, BuyerId>(order.GetBuyerId, CancellationToken.None)
                .ConfigureAwait(false);

            Console.WriteLine("BuyerId: " + order.GetBuyerId);
            Console.WriteLine(JsonConvert.SerializeObject(buyer));

            var orderStatusChangedToStockConfirmedIntegrationEvent = new OrderStatusChangedToStockConfirmedIntegrationEvent(order.Id.Value, order.OrderStatus.Name, order.GetBuyerId.Value, "bob", order.OrderItems.Sum( x => x.UnitPrice * x.Units));
            await _endpoint.Publish(orderStatusChangedToStockConfirmedIntegrationEvent);
        }
    }
}