using EventFlow;
using EventFlow.Configuration;
using EventFlow.EntityFramework;
using EventFlow.EntityFramework.Extensions;
using EventFlow.Extensions;
using EventFlow.Queries;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
using System.Threading;
using Ordering.ReadModel.Model;
using Ordering.ReadModel.Queries;
using Ordering.ReadModel.QueryHandler;

namespace Ordering.ReadModel
{
    public static class ReadModelConfiguration
    {
        public static IEventFlowOptions AddEntityFrameworkReadModel(this IEventFlowOptions efo)
        {
            var queries = new[] {
                typeof(GetOrderQueryHandler),
                typeof(GetOrdersFromUserQueryHandler)
            };

            return efo
                .UseEntityFrameworkReadModel<OrderReadModel, OrderingDbContext>()
                .UseEntityFrameworkEventStore<OrderingDbContext>()
                .AddQueryHandlers(queries)
                .RegisterServices(sr => sr.Register(c => @"Server=tcp:127.0.0.1,5433;Initial Catalog=CapacitacionMicroservicios.OrderingDb;User Id=sa;Password=Pass@word"))
                .ConfigureEntityFramework(EntityFrameworkConfiguration.New)
                .AddDbContextProvider<OrderingDbContext, DbContextProvider>();
        }
    }
}
