using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.BuyerAggregate.Identity;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
using EventFlow.Aggregates;

namespace Ordering.Domain.Events
{
    public class BuyerCreatedDomainEvent : IAggregateEvent<Buyer, BuyerId>
    {
        public BuyerCreatedDomainEvent(string identityGuid, string buyerName)
        {
            this.IdentityGuid = identityGuid;
            this.BuyerName = buyerName;

        }
        public string IdentityGuid { get; private set; }
        public string BuyerName { get; private set; }
    }
}
