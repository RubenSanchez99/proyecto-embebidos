using System;
using EventFlow.Core;
using EventFlow.ValueObjects;
using Newtonsoft.Json;

namespace Ordering.Domain.AggregatesModel.OrderAggregate.Identity
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class OrderItemId : Identity<OrderItemId>
    {
        public OrderItemId(string value) : base(value)
        {
        }
    }
}
