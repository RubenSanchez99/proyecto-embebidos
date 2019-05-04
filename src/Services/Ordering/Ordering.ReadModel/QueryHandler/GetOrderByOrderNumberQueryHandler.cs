using System.Threading;
using System.Threading.Tasks;
using EventFlow.Queries;
using Ordering.ReadModel.Queries;
using EventFlow.EntityFramework;
using System.Linq;
using Ordering.ReadModel.Model;

namespace Ordering.ReadModel.QueryHandler
{
    public class GetOrderByOrderNumberQueryHandler : IQueryHandler<GetOrderByOrderNumberQuery, OrderReadModel>
    {
        private readonly IDbContextProvider<OrderingDbContext> _dbContextProvider;

        public GetOrderByOrderNumberQueryHandler(IDbContextProvider<OrderingDbContext> dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public Task<OrderReadModel> ExecuteQueryAsync(GetOrderByOrderNumberQuery query, CancellationToken cancellationToken)
        {
            using (var context = _dbContextProvider.CreateContext())
            {
                var data = context.Orders.SingleOrDefault(x => x.OrderNumber == query.OrderNumber);
                return Task.FromResult(data);
            };
        }
    }
}