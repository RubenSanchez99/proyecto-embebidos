using System;
using System.Collections.Generic;
using EventFlow.Aggregates.ExecutionResults;
using Ordering.Domain.AggregatesModel.OrderAggregate;

namespace Ordering.Domain.ExecutionResults
{
    public class OrderDraftExcecutionResult : IExecutionResult
    {
        public OrderDraftExcecutionResult(bool isSuccess, OrderDraft order)
        {
            this.IsSuccess = isSuccess;
            this.Order = order;

        }
        public bool IsSuccess { get; }

        public OrderDraft Order { get; }
    }

    public class OrderDraft
    {
        public int OrderNumber { get; set; }

        public DateTime Date { get; set; }

        public string Status { get; set; }

        public decimal Total { get; set; }

        public string Description { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }

        // See the property initializer syntax below. This
        // initializes the compiler generated field for this
        // auto-implemented property.
        public List<OrderDraftItem> OrderItems { get; set; } = new List<OrderDraftItem>();
    }

    public class OrderDraftItem
    {
        public string ProductName { get; set; }
        public OrderDraftItem(string productName, decimal unitPrice, int units, string pictureUrl)
        {
            this.ProductName = productName;
            this.UnitPrice = unitPrice;
            this.Units = units;
            this.PictureUrl = pictureUrl;

        }

        public decimal UnitPrice { get; set; }

        public int Units { get; set; }

        public string PictureUrl { get; set; }
    }
}