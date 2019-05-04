namespace Payment.API.IntegrationEvents.EventHandling
{
    using MassTransit;
    using Microsoft.Extensions.Options;
    using eShopOnContainers.Services.IntegrationEvents.Events;
    using System.Threading.Tasks;
    using Payment.API.Infrastructure;
    using System.Linq;
    using System;

    public class OrderStatusChangedToStockConfirmedIntegrationEventHandler : 
        IConsumer<OrderStatusChangedToStockConfirmedIntegrationEvent>
    {
        private readonly IPublishEndpoint _endpoint;
        private readonly PaymentSettings _settings;
        private readonly PaymentContext _context;

        public OrderStatusChangedToStockConfirmedIntegrationEventHandler(IPublishEndpoint endpoint, 
            IOptionsSnapshot<PaymentSettings> settings, PaymentContext context)
        {
            _endpoint = endpoint;
            _settings = settings.Value;
            _context = context;
        }

        public async Task Consume(ConsumeContext<OrderStatusChangedToStockConfirmedIntegrationEvent> context)
        {
            Guid buyerId = new Guid(context.Message.BuyerId.Substring(6));
            
            var buyer = _context.Accounts.SingleOrDefault(x => x.BuyerId == buyerId);

            if (buyer != null && buyer.AmountAvailable >= context.Message.Amount)
            {
                var user = _context.Accounts.Single( x=> x.BuyerId == buyerId);
                user.AmountAvailable = user.AmountAvailable - context.Message.Amount;
                _context.SaveChanges();
                await _endpoint.Publish(new OrderPaymentSuccededIntegrationEvent(context.Message.OrderId));
            }
            else
            {
                await _endpoint.Publish(new OrderPaymentFailedIntegrationEvent(context.Message.OrderId));
            }
            
            await Task.CompletedTask;
        }
    }
}