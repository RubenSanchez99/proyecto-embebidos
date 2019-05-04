using System;
using Automatonymous;
using eShopOnContainers.Services.IntegrationEvents.Events;
using EventFlow;
using Ordering.API.Application.Commands;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using EventFlow.Core;
using System.Collections.Generic;
using System.Linq;
using Ordering.API.Extensions;

namespace Ordering.API.Application.Sagas
{
    public class GracePeriodStateMachine :
    MassTransitStateMachine<GracePeriod>
    {
        private readonly IAggregateStore _aggregateStore;
        public GracePeriodStateMachine(IAggregateStore aggregateStore)
        {
            _aggregateStore = aggregateStore ?? throw new ArgumentNullException(nameof(aggregateStore));;

            InstanceState(x => x.CurrentState);

            Event(() => OrderStarted, x => x.CorrelateBy(gracePeriod => gracePeriod.OrderId, context => context.Message.OrderId).SelectId(context => Guid.NewGuid()));

            Event(() => OrderStockConfirmed, x => x.CorrelateBy(gracePeriod => gracePeriod.OrderId, context => context.Message.OrderId));

            Event(() => OrderStockRejected, x => x.CorrelateBy(gracePeriod => gracePeriod.OrderId, context => context.Message.OrderId));

            Event(() => OrderPaymentSucceded, x => x.CorrelateBy(gracePeriod => gracePeriod.OrderId, context => context.Message.OrderId));

            Event(() => OrderPaymentFailed, x => x.CorrelateBy(gracePeriod => gracePeriod.OrderId, context => context.Message.OrderId));

            Event(() => OrderCanceled, x => x.CorrelateBy(gracePeriod => gracePeriod.OrderId, context => context.Message.OrderId));

            Event(() => OrderStockSent, x => x.CorrelateBy(gracePeriod => gracePeriod.OrderId, context => context.Message.OrderId));

            Schedule(() => GracePeriodFinished, x => x.ExpirationId, x => 
            {
                x.Delay = TimeSpan.FromMinutes(5);
                x.Received = e => e.CorrelateBy(gracePeriod => gracePeriod.OrderId, context => context.Message.OrderId);
            });

            Initially(
                When(OrderStarted)
                    .Then(context => context.Instance.OrderId = context.Data.OrderId)
                    .Then(context => context.Instance.OrderIdentity = new OrderId(context.Data.OrderId))
                    .Then(context => context.Instance.SourceId = new SourceId(context.Data.RequestId.ToString()))
                    .Publish(context => new GracePeriodConfirmedIntegrationEvent(context.Data.OrderId))
                    .ThenAsync(context =>
                        SetAwaitingValidationStatus(context.Instance.OrderIdentity, context.CreateConsumeContext().MessageId.Value.ToSourceId())
                    )
                    .Schedule(GracePeriodFinished, context => new GracePeriodExpired(context.Data.OrderId))
                    .TransitionTo(AwaitingValidation)
            );

            During(AwaitingValidation,
                When(OrderStockConfirmed)
                    .ThenAsync(context => SetStockConfirmedStatusAsync(context.Instance.OrderIdentity, context.CreateConsumeContext().MessageId.Value.ToSourceId()))
                    .TransitionTo(StockConfirmed),
                When(OrderStockRejected)
                    .Unschedule(GracePeriodFinished)
                    .ThenAsync(context => SetCancelledStatusWhenStockIsRejectedAsync(context.Instance.OrderIdentity, context.Data.OrderStockItems, context.CreateConsumeContext().MessageId.Value.ToSourceId()))
                    .TransitionTo(Failed),
                When(GracePeriodFinished.Received)
                    .TransitionTo(Failed)
            );

            During(StockConfirmed,
                When(OrderPaymentSucceded)
                    .Unschedule(GracePeriodFinished)
                    .ThenAsync(context => SetPaidStatusAsync(context.Instance.OrderIdentity, context.CreateConsumeContext().MessageId.Value.ToSourceId()))
                    .TransitionTo(Validated),
                When(OrderPaymentFailed)
                    .Unschedule(GracePeriodFinished)
                    .ThenAsync(context => SetCancelledStatusAsync(context.Instance.OrderIdentity, context.CreateConsumeContext().MessageId.Value.ToSourceId()))
                    .TransitionTo(Failed),
                When(GracePeriodFinished.Received)
                    .TransitionTo(Failed)
            );

            During(PaymentSucceded,
                When(OrderStockConfirmed)
                    .Unschedule(GracePeriodFinished)
                    .ThenAsync(context => SetStockConfirmedStatusAsync(context.Instance.OrderIdentity, context.CreateConsumeContext().MessageId.Value.ToSourceId()))
                    .TransitionTo(Validated),
                When(OrderStockRejected)
                    .Unschedule(GracePeriodFinished)
                    .ThenAsync(context => SetCancelledStatusWhenStockIsRejectedAsync(context.Instance.OrderIdentity, context.Data.OrderStockItems, context.CreateConsumeContext().MessageId.Value.ToSourceId()))
                    .Finalize(),
                When(GracePeriodFinished.Received)
                    .TransitionTo(Failed)
            );

            During(Validated,
                When(OrderStockSent)
                    .ThenAsync(context => ShipOrderAsync(context.Instance.OrderIdentity, context.CreateConsumeContext().MessageId.Value.ToSourceId()))
                    .Finalize()
            );


            WhenEnter(Failed, 
                x => x.ThenAsync(context => SetCancelledStatusAsync(context.Instance.OrderIdentity, context.CreateConsumeContext().MessageId.Value.ToSourceId()))
                      .Finalize()
            );

            During(Final,
                Ignore(GracePeriodFinished.AnyReceived)
            );
        }

        private async Task SetAwaitingValidationStatus(OrderId orderId, ISourceId sourceId)
        {            
            await _aggregateStore.UpdateAsync<Order, OrderId>(orderId, sourceId,
                (order, c) => {
                        order.SetAwaitingValidationStatus();
                        return Task.FromResult(0);
                }, CancellationToken.None
            ).ConfigureAwait(false);
        }

        private async Task SetStockConfirmedStatusAsync(OrderId orderId, ISourceId sourceId)
        {            
            await _aggregateStore.UpdateAsync<Order, OrderId>(orderId, sourceId,
                (order, c) => {
                        order.SetStockConfirmedStatus();
                        return Task.FromResult(0);
                }, CancellationToken.None
            ).ConfigureAwait(false);
        }

        private async Task SetCancelledStatusWhenStockIsRejectedAsync(OrderId orderId, List<ConfirmedOrderStockItem> orderStockItems, ISourceId sourceId)
        {
            var orderStockRejectedItems = orderStockItems
                .FindAll(c => !c.HasStock)
                .Select(c => c.ProductId);

            await _aggregateStore.UpdateAsync<Order, OrderId>(orderId, sourceId,
                (order, c) => {
                        order.SetCancelledStatusWhenStockIsRejected(orderStockRejectedItems);
                        return Task.FromResult(0);
                }, CancellationToken.None
            ).ConfigureAwait(false);
        }

        private async Task SetPaidStatusAsync(OrderId orderId, ISourceId sourceId)
        {
            await _aggregateStore.UpdateAsync<Order, OrderId>(orderId, sourceId,
                (order, c) => {
                        order.SetPaidStatus();
                        return Task.FromResult(0);
                }, CancellationToken.None
            ).ConfigureAwait(false);
        }

        private async Task SetCancelledStatusAsync(OrderId orderId, ISourceId sourceId)
        {
            await _aggregateStore.UpdateAsync<Order, OrderId>(orderId, sourceId,
                (order, c) => {
                        order.SetCancelledStatus();
                        return Task.FromResult(0);
                }, CancellationToken.None
            ).ConfigureAwait(false);
        }

        private async Task ShipOrderAsync(OrderId orderId, ISourceId sourceId)
        {
            await _aggregateStore.UpdateAsync<Order, OrderId>(orderId, sourceId,
                (order, c) => {
                        order.SetShippedStatus();
                        return Task.FromResult(0);
                }, CancellationToken.None
            ).ConfigureAwait(false);
        }

        public Event<OrderStartedIntegrationEvent> OrderStarted { get; private set; }
        public Event<OrderStockConfirmedIntegrationEvent> OrderStockConfirmed { get; private set; }
        public Event<OrderStockRejectedIntegrationEvent> OrderStockRejected { get; private set; }
        public Event<OrderPaymentSuccededIntegrationEvent> OrderPaymentSucceded { get; private set; }
        public Event<OrderPaymentFailedIntegrationEvent> OrderPaymentFailed { get; private set; }
        public Event<OrderStatusChangedToCancelledIntegrationEvent> OrderCanceled { get; private set; }
        public Event<OrderStockSentForOrderIntegrationEvent> OrderStockSent { get; private set; }
        
        public Schedule<GracePeriod, GracePeriodExpired> GracePeriodFinished { get; private set; }

        public State AwaitingValidation { get; private set; }
        public State StockConfirmed { get; private set; }
        public State PaymentSucceded { get; private set; }
        public State Validated { get; private set; }
        public State Failed { get; private set; }
        
        public class GracePeriodExpired
        {
            public string OrderId { get; private set; }
            
            public GracePeriodExpired(string orderId) => OrderId = orderId;
        }
    }
}
