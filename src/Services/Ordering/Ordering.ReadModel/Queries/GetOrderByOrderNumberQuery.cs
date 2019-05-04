using EventFlow.Queries;
using Ordering.ReadModel.Model;

namespace Ordering.ReadModel.Queries
{
    public class GetOrderByOrderNumberQuery : IQuery<OrderReadModel>
    {
        public GetOrderByOrderNumberQuery(int orderNumber)
        {
            this.OrderNumber = orderNumber;

        }
        public int OrderNumber { get; private set; }
    }
}