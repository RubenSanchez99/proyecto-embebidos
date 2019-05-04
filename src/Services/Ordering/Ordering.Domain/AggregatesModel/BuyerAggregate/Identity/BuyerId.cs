using System;
using EventFlow.Core;
using EventFlow.ValueObjects;
using Newtonsoft.Json;

namespace Ordering.Domain.AggregatesModel.BuyerAggregate.Identity
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class BuyerId : Identity<BuyerId>
    {
        public BuyerId(string value) : base(value)
        {
        }
    }
}
