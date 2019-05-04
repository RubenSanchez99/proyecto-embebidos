using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;

namespace Ordering.API.Application.Commands
{
    public class CancelOrderCommandHandler : CommandHandler<Order, OrderId, IExecutionResult, CancelOrderCommand>
    {
        public override Task<IExecutionResult> ExecuteCommandAsync(Order aggregate, CancelOrderCommand command, CancellationToken cancellationToken)
        {
            var excecutionResult = aggregate.SetCancelledStatus();
            return Task.FromResult(excecutionResult);
        }
    }
}
