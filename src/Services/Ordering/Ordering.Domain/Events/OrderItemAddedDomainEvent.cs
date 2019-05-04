using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
using EventFlow.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.Events
{
    public class OrderItemAddedDomainEvent : IAggregateEvent<Order, OrderId>
    {
        public OrderItemAddedDomainEvent(int productId, string productName, string pictureUrl, decimal unitPrice, decimal discount, int units)
        {
            this.ProductId = productId;
            this.ProductName = productName;
            this.PictureUrl = pictureUrl;
            this.UnitPrice = unitPrice;
            this.Discount = discount;
            this.Units = units;

        }

        public int ProductId { get; }
        public string ProductName { get; }
        public string PictureUrl { get; }
        public decimal UnitPrice { get; }
        public decimal Discount { get; }
        public int Units { get; }
    }
}
