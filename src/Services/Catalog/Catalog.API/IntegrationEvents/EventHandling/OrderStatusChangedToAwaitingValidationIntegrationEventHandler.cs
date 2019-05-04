using eShopOnContainers.Services.IntegrationEvents.Events;
using MassTransit;

namespace Catalog.API.IntegrationEvents.EventHandling
{
    using System.Threading.Tasks;
    using Catalog.API.Infrastructure;
    using System.Collections.Generic;
    using System.Linq;

    public class OrderStatusChangedToAwaitingValidationIntegrationEventHandler : 
        IConsumer<OrderStatusChangedToAwaitingValidationIntegrationEvent>
    {
        private readonly CatalogContext _catalogContext;

        public OrderStatusChangedToAwaitingValidationIntegrationEventHandler(CatalogContext catalogContext)
        {
            _catalogContext = catalogContext;
        }

        public async Task Consume(ConsumeContext<OrderStatusChangedToAwaitingValidationIntegrationEvent> context)
        {
            var confirmedOrderStockItems = new List<ConfirmedOrderStockItem>();

            foreach (var orderStockItem in context.Message.OrderStockItems)
            {
                var catalogItem = _catalogContext.CatalogItems.Find(orderStockItem.ProductId);
                var hasStock = catalogItem.AvailableStock >= orderStockItem.Units;
                var confirmedOrderStockItem = new ConfirmedOrderStockItem(catalogItem.Id, hasStock);

                confirmedOrderStockItems.Add(confirmedOrderStockItem);
            }

            if (confirmedOrderStockItems.Any(c => !c.HasStock))
            {
                await context.Publish(new OrderStockRejectedIntegrationEvent(context.Message.OrderId, confirmedOrderStockItems));
            }
            else
            {
                await context.Publish(new OrderStockConfirmedIntegrationEvent(context.Message.OrderId));
            }
        }
    }
}