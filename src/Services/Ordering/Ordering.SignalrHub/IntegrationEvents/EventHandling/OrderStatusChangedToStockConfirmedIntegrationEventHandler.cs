using Microsoft.AspNetCore.SignalR;
using eShopOnContainers.Services.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;

namespace Ordering.SignalrHub.IntegrationEvents.EventHandling
{
    public class OrderStatusChangedToStockConfirmedIntegrationEventHandler :
        IConsumer<OrderStatusChangedToStockConfirmedIntegrationEvent>
    {
        private readonly IHubContext<NotificationsHub> _hubContext;

        public OrderStatusChangedToStockConfirmedIntegrationEventHandler(IHubContext<NotificationsHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        public async Task Consume(ConsumeContext<OrderStatusChangedToStockConfirmedIntegrationEvent> context)
        {
            await _hubContext.Clients
                .Group(context.Message.BuyerName)
                .SendAsync("UpdatedOrderState", new { OrderId = context.Message.OrderId, Status = context.Message.OrderStatus });
        }
    }
}
