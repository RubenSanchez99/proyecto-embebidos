namespace eShopOnContainers.Services.IntegrationEvents.Events
{
    public interface ProductPriceChangedIntegrationEvent
    {
        int ProductId { get; }

        decimal NewPrice { get; }

        decimal OldPrice { get; }
    }
}