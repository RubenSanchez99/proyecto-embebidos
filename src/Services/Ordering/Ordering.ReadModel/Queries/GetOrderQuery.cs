using EventFlow.Queries;
using Ordering.ReadModel.Model;

namespace Ordering.ReadModel.Queries
{
    public class GetOrderQuery : IQuery<OrderReadModel>
    {
        public GetOrderQuery(string id)
        {
            this.id = id;

        }
        public string id { get; private set; }
    }
}
