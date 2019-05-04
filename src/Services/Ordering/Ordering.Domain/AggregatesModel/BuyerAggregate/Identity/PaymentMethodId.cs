using System;
using EventFlow.Core;
using EventFlow.ValueObjects;
using Newtonsoft.Json;

namespace Ordering.Domain.AggregatesModel.BuyerAggregate.Identity
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class PaymentMethodId : Identity<PaymentMethodId>
    {
        public PaymentMethodId(string value) : base(value)
        {
        }
    }
}
