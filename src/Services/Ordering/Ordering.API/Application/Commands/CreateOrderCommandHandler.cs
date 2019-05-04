using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;

namespace Ordering.API.Application.Commands
{
    public class CreateOrderCommandHandler : CommandHandler<Order, OrderId, IExecutionResult, CreateOrderCommand>
    {
        public override Task<IExecutionResult> ExecuteCommandAsync(Order aggregate, CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var address = new Address(command.Street, command.City, command.State, command.Country, command.ZipCode);

            Console.Out.WriteLine("OrderId = " + aggregate.Id.ToString() );
            Console.Out.WriteLine();

            var items = from tItem in command.OrderItems
                select new OrderItem(OrderItemId.New, tItem.ProductId, tItem.ProductName, tItem.UnitPrice, tItem.Discount, tItem.PictureUrl, tItem.Units);

            var executionResult = aggregate.Create(command.UserId, command.UserName, address, command.CardTypeId, command.CardNumber, command.CardSecurityNumber, command.CardHolderName, command.CardExpiration, items);

            Console.Out.WriteLine("Order created");

            //foreach (var item in command.OrderItems)
            //{
            //    aggregate.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.Discount, "", item.Units);
            //}

            return Task.FromResult(executionResult);
        }
    }
}
