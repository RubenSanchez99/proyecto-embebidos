using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.BuyerAggregate.Identity;
using EventFlow.Aggregates;
using System;

namespace Ordering.Domain.Events
{
    public class BuyerPaymentMethodAddedDomainEvent : IAggregateEvent<Buyer, BuyerId>
    {
        public PaymentMethodId PaymentMethodId { get; }
        public string Alias { get; }
        public string CardNumber { get; }
        public string SecurityNumber { get; }
        public string CardHolderName { get; }
        public DateTime Expiration { get; }

        public int CardTypeId;
        public BuyerPaymentMethodAddedDomainEvent(PaymentMethodId paymentMethodId, int cardTypeId, string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration)
        {
            this.PaymentMethodId = paymentMethodId;
            this.Alias = alias;
            this.CardNumber = cardNumber;
            this.CardTypeId = cardTypeId;
            this.SecurityNumber = securityNumber;
            this.Expiration = expiration;
            this.CardHolderName = cardHolderName;
        }
    }
}
