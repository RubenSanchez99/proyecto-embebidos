using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using eShopOnContainers.Services.IntegrationEvents.Events;
using MassTransit;
using EventFlow;
using EventFlow.Aggregates;
using EventFlow.Aggregates.ExecutionResults;
using Ordering.API.Application.Commands;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
using Microsoft.Extensions.Logging;
using EventFlow.Core;

namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    public class UserCheckoutAcceptedIntegrationEventHandler : IConsumer<UserCheckoutAcceptedIntegrationEvent>
    {
        private readonly ICommandBus _commandBus;
        private readonly IPublishEndpoint _endpoint;
        private readonly ILogger<UserCheckoutAcceptedIntegrationEventHandler> _log;
        public UserCheckoutAcceptedIntegrationEventHandler(IPublishEndpoint endpoint, ICommandBus commandBus, ILogger<UserCheckoutAcceptedIntegrationEventHandler> log)
        {
            _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            _commandBus = commandBus ?? throw new ArgumentNullException(nameof(commandBus));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task Consume(ConsumeContext<UserCheckoutAcceptedIntegrationEvent> context)
        {
            IExecutionResult result = ExecutionResult.Failed();
            var orderId = OrderId.New;

            // Send Integration event to clean basket once basket is converted to Order and before starting with the order creation process
            var orderItems = from orderItem in context.Message.Basket.Items
                select new OrderStockItem(int.Parse(orderItem.ProductId), orderItem.Quantity);

            if (context.Message.RequestId != Guid.Empty)
            {
                var createOrderCommand = new CreateOrderCommand(orderId, new SourceId(context.Message.RequestId.ToString()), context.Message.Basket.Items, context.Message.UserId, context.Message.UserName, context.Message.City, context.Message.Street, 
                    context.Message.State, context.Message.Country, context.Message.ZipCode,
                    context.Message.CardNumber, context.Message.CardHolderName, context.Message.CardExpiration,
                    context.Message.CardSecurityNumber, context.Message.CardTypeId);

                result = await _commandBus.PublishAsync(createOrderCommand, CancellationToken.None).ConfigureAwait(false);
                
                if (result.IsSuccess)
                {
                    var orderStartedIntegrationEvent = new OrderStartedIntegrationEvent(context.Message.UserId, orderId.Value, orderItems.ToList(), context.Message.RequestId);
                    await _endpoint.Publish(orderStartedIntegrationEvent);
                }
            }
        }
    }
}
