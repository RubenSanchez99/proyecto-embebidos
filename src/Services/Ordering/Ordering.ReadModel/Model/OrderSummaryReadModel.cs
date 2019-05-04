using System;
using System.ComponentModel.DataAnnotations;
using EventFlow.Aggregates;
using EventFlow.ReadStores;
using Ordering.Domain.Events;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
using Ordering.Domain.AggregatesModel.OrderAggregate;

namespace Ordering.ReadModel.Model
{
    public class OrderSummaryReadModel
    {
        [Key]
        public int OrderNumber { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public decimal Total { get; set; }
    }
}
