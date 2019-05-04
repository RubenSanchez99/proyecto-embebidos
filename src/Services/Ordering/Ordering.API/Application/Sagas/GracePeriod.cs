using System;
using Automatonymous;
using EventFlow.Core;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;

namespace Ordering.API.Application.Sagas
{
    public class GracePeriod : SagaStateMachineInstance
    {
        public string CurrentState { get; set; }
        public Guid CorrelationId { get ; set; }
        public string OrderId { get; set; }
        public OrderId OrderIdentity { get; set; }
        public SourceId SourceId { get; set; }
        public Guid? ExpirationId { get; set; }
    }
}