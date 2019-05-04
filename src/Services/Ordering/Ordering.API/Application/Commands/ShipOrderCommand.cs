using System.Runtime.Serialization;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
using EventFlow.Commands;

namespace Ordering.API.Application.Commands
{
    public class ShipOrderCommand : Command<Order, OrderId>
    {
        [DataMember]
        public int OrderNumber { get; private set; }

        public ShipOrderCommand(OrderId id, int orderNumber) : base(id)
        {
            OrderNumber = orderNumber;
        }
    }
}
