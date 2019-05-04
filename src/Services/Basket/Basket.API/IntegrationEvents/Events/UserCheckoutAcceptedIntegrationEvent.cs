using Basket.API.Model;
using System;

namespace eShopOnContainers.Services.IntegrationEvents.Events
{
    public interface UserCheckoutAcceptedIntegrationEvent
    {
        string UserId { get; }

        string UserName { get; }

        string City { get; set; }

        string Street { get; set; }

        string State { get; set; }

        string Country { get; set; }

        string ZipCode { get; set; }

        string CardNumber { get; set; }

        string CardHolderName { get; set; }

        DateTime CardExpiration { get; set; }

        string CardSecurityNumber { get; set; }

        int CardTypeId { get; set; }

        string Buyer { get; set; }

        Guid RequestId { get; set; }

        CustomerBasket Basket { get; }
    }
}
