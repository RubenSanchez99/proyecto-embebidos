using System.Collections.Generic;
using System.Runtime.Serialization;
using Ordering.API.Application.Models;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
using Ordering.Domain.ExecutionResults;
using System.Linq;
using EventFlow.Commands;

namespace Ordering.API.Application.Commands
{
    public class CreateOrderDraftCommand : Command<Order, OrderId, OrderDraftExcecutionResult>
    {
        public string BuyerId { get; private set; }

        public IEnumerable<BasketItem> Items { get; private set; }

        public CreateOrderDraftCommand(OrderId id, string buyerId, IEnumerable<BasketItem> items) : base(id)
        {
            BuyerId = buyerId;
            Items = items;
        }
    }
}
