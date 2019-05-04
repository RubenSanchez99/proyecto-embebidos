using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
using Ordering.Domain.ExecutionResults;
using System.Linq;

namespace Ordering.API.Application.Commands
{
    public class CreateOrderDraftCommandHandler : CommandHandler<Order, OrderId, OrderDraftExcecutionResult, CreateOrderDraftCommand>
    {
        public override Task<OrderDraftExcecutionResult> ExecuteCommandAsync(Order aggregate, CreateOrderDraftCommand command, CancellationToken cancellationToken)
        {
            var items = from tItem in command.Items
                        select new OrderItem(OrderItemId.New, int.Parse(tItem.ProductId), tItem.ProductName, tItem.UnitPrice, 0, tItem.PictureUrl, tItem.Quantity);

            var result = aggregate.NewDraft(command.BuyerId, items);
            return Task.FromResult(result);
        }
    }
}
