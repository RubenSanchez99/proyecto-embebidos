using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EventFlow.Queries;
using Ordering.ReadModel.Queries;
using Ordering.ReadModel.Model;
using EventFlow.EntityFramework;

namespace Ordering.ReadModel.QueryHandler
{
    public class GetOrdersFromUserQueryHandler : IQueryHandler<GetOrdersFromUserQuery, List<OrderSummaryReadModel>>
    {
        private readonly IDbContextProvider<OrderingDbContext> _dbContextProvider;

        public GetOrdersFromUserQueryHandler(IDbContextProvider<OrderingDbContext> dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public async Task<List<OrderSummaryReadModel>> ExecuteQueryAsync(GetOrdersFromUserQuery query, CancellationToken cancellationToken)
        {
            using (var context = _dbContextProvider.CreateContext())
            {
                var data = 
                    from tOrder in context.Orders
                    where tOrder.BuyerIdentityGuid == ("buyer-" + query.userId.ToString())
                    select new OrderSummaryReadModel
                    {
                        OrderNumber = tOrder.OrderNumber,
                        Date = tOrder.Date,
                        Status = tOrder.Status,
                        Total = tOrder.Total
                    };
                    
                return await data.ToListAsync();
            }
        }
    }
}
