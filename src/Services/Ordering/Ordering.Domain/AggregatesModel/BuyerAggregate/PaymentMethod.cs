using System;
using EventFlow.Entities;
using Newtonsoft.Json;
using Ordering.Domain.AggregatesModel.BuyerAggregate.Identity;
using Ordering.Domain.Exceptions;

namespace Ordering.Domain.AggregatesModel.BuyerAggregate
{
    public class PaymentMethod : Entity<PaymentMethodId>
    {
        private string _alias;
        private string _cardNumber;
        private string _securityNumber;
        private string _cardHolderName;
        private DateTime _expiration;

        private int _cardTypeId;
        public CardType CardType { get; private set; }

        [JsonConstructor]
        public PaymentMethod(PaymentMethodId id) : base(id)
        {

        }

        public PaymentMethod(PaymentMethodId id, int cardTypeId, string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration) : base(id)
        {
            Console.Out.WriteLine($"PaymentMethodId: {id.Value}, cardTypeId: {cardTypeId}, alias: {alias}, cardNumber: {cardNumber}, securityNumber: {securityNumber}, cardHolderName: {cardHolderName}, expiration: {expiration}");
            //_cardNumber = cardNumber == null ? cardNumber : throw new OrderingDomainException(nameof(cardNumber));
            //_securityNumber = securityNumber == null ? securityNumber : throw new OrderingDomainException(nameof(securityNumber));
            //_cardHolderName = cardHolderName == null ? cardHolderName : throw new OrderingDomainException(nameof(cardHolderName));

            _cardNumber = cardNumber;
            _securityNumber = securityNumber;
            _cardHolderName = cardHolderName;

            /* if (expiration < DateTime.UtcNow)
            {
                throw new OrderingDomainException(nameof(expiration));
            }*/

            _alias = alias;
            _expiration = expiration;
            _cardTypeId = cardTypeId;
        }
        
        public bool IsEqualTo(int cardTypeId, string cardNumber,DateTime expiration)
        {
            return _cardTypeId == cardTypeId
                && _cardNumber == cardNumber
                && _expiration == expiration;
        }
    }
}
