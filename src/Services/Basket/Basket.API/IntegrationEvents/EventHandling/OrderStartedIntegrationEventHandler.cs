using Basket.API.Model;
using System;
using System.Threading.Tasks;
using eShopOnContainers.Services.IntegrationEvents.Events;
using MassTransit;

namespace Basket.API.IntegrationEvents.EventHandling
{
    public class OrderStartedIntegrationEventHandler : IConsumer<OrderStartedIntegrationEvent>
    {
        private readonly IBasketRepository _repository;

        public OrderStartedIntegrationEventHandler(IBasketRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task Consume(ConsumeContext<OrderStartedIntegrationEvent> context)
        {
            await _repository.DeleteBasketAsync(context.Message.UserId.ToString());
        }
    }
}
