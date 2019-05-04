using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;

namespace Ordering.API.Application.Commands
{
    public class ShipOrderCommandHandler : CommandHandler<Order, OrderId, IExecutionResult, ShipOrderCommand>
    {
        public override Task<IExecutionResult> ExecuteCommandAsync(Order aggregate, ShipOrderCommand command, CancellationToken cancellationToken)
        {
            var excecutionResult = aggregate.SetShippedStatus();
            return Task.FromResult(excecutionResult);
        }
    }
}
