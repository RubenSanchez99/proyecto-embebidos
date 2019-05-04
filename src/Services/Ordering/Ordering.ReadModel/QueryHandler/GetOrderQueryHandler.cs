using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using EventFlow.Queries;
using EventFlow.Aggregates;
using Ordering.ReadModel.Model;
using Ordering.ReadModel.Queries;
using EventFlow.EntityFramework;

namespace Ordering.ReadModel.QueryHandler
{
    public class GetOrderQueryHandler : IQueryHandler<GetOrderQuery, OrderReadModel>
    {
        private readonly IDbContextProvider<OrderingDbContext> _dbContextProvider;

        public GetOrderQueryHandler(IDbContextProvider<OrderingDbContext> dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public Task<OrderReadModel> ExecuteQueryAsync(GetOrderQuery query, CancellationToken cancellationToken)
        {
            using (var context = _dbContextProvider.CreateContext())
            {
                var data = context.Orders.SingleOrDefault(x => x.OrderNumber.ToString() == query.id);
                return Task.FromResult(data);
            };
        }
    }
}
