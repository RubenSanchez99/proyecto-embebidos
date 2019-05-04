using eShopOnContainers.Services.IntegrationEvents.Events;
using MassTransit;

namespace Catalog.API.IntegrationEvents.EventHandling
{
    using System.Threading.Tasks;
    using Infrastructure;

    public class OrderStatusChangedToPaidIntegrationEventHandler : 
        IConsumer<OrderStatusChangedToPaidIntegrationEvent>
    {
        private readonly CatalogContext _catalogContext;
        private readonly IPublishEndpoint _endpoint;

        public OrderStatusChangedToPaidIntegrationEventHandler(CatalogContext catalogContext, IPublishEndpoint endpoint)
        {
            _catalogContext = catalogContext;
            _endpoint = endpoint;
        }

        public async Task Consume(ConsumeContext<OrderStatusChangedToPaidIntegrationEvent> context)
        {
            //we're not blocking stock/inventory
            foreach (var orderStockItem in context.Message.OrderStockItems)
            {
                var catalogItem = _catalogContext.CatalogItems.Find(orderStockItem.ProductId);

                catalogItem.RemoveStock(orderStockItem.Units);
            }

            await _catalogContext.SaveChangesAsync();

            var orderStockSentForOrder = new OrderStockSentForOrderIntegrationEvent(context.Message.OrderId);
            await _endpoint.Publish(orderStockSentForOrder);
        }
    }
}