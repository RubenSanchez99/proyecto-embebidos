using System;
using EventFlow.Core;
using EventFlow.ValueObjects;
using Newtonsoft.Json;

namespace Ordering.Domain.AggregatesModel.OrderAggregate.Identity
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class OrderId : Identity<OrderId>
    {
        public OrderId(string value) : base(value)
        {
        }
    }
}
