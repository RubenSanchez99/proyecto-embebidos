using System;
using System.Collections.Generic;
using EventFlow.Queries;
using Ordering.ReadModel.Model;

namespace Ordering.ReadModel.Queries
{
    public class GetOrdersFromUserQuery : IQuery<List<OrderSummaryReadModel>>
    {
        public GetOrdersFromUserQuery(Guid userId)
        {
            this.userId = userId;

        }
        public Guid userId { get; set; }
    }
}
