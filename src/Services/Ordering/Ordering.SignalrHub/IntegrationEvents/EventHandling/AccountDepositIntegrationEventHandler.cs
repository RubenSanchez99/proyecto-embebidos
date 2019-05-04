using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShopOnContainers.Services.IntegrationEvents.Events;
using MassTransit;

namespace Ordering.SignalrHub.IntegrationEvents
{
    public class AccountDepositIntegrationEventHandler : IConsumer<AccountDepositIntegrationEvent>
    {
        private readonly IHubContext<NotificationsHub> _hubContext;

        public AccountDepositIntegrationEventHandler(IHubContext<NotificationsHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        public async Task Consume(ConsumeContext<AccountDepositIntegrationEvent> context)
        {
            await _hubContext.Clients
                .Group(context.Message.BuyerName)
                .SendAsync("UpdatedAvailableAmount", new { DepositAmount = context.Message.DepositAmount, NewAmount = context.Message.DepositAmount + context.Message.PreviousAmount });
        }
    }
}