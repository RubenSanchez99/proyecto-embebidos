using System;
using EventFlow.Entities;
using Ordering.Domain.Exceptions;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
using Newtonsoft.Json;

namespace Ordering.Domain.AggregatesModel.OrderAggregate
{
    public class OrderItem : Entity<OrderItemId>
    {
        // DDD Patterns comment
        // Using private fields, allowed since EF Core 1.1, is a much better encapsulation
        // aligned with DDD Aggregates and Domain Entities (Instead of properties and property collections)
        public readonly string  ProductName;
        public readonly string  PictureUrl;
        public readonly decimal UnitPrice;
        public decimal Discount { get; private set; }
        public int     Units { get; private set; }

        public readonly int ProductId;

        public OrderItem(OrderItemId id, int productId, string productName, decimal unitPrice, decimal discount, string pictureUrl, int units = 1) : base(id)
        {
            if (units <= 0)
            {
                throw new OrderingDomainException("Invalid number of units");
            }

            if ((unitPrice * units) < discount)
            {
                throw new OrderingDomainException("The total of order item is lower than applied discount");
            }

            ProductId = productId;

            ProductName = productName;
            UnitPrice = unitPrice;
            Discount = discount;
            Units = units;
            PictureUrl = pictureUrl;
        }
/*
        public string GetPictureUri() => _pictureUrl;

        public decimal GetCurrentDiscount()
        {
            return _discount;
        }

        public int GetUnits()
        {
            return _units;
        }

        public decimal GetUnitPrice()
        {
            return _unitPrice;
        }

        public string GetOrderItemProductName() => _productName;
*/
        public void SetNewDiscount(decimal discount)
        {
            if (discount < 0)
            {
                throw new OrderingDomainException("Discount is not valid");
            }

            Discount = discount;
        }

        public void AddUnits(int units)
        {
            if (units < 0)
            {
                throw new OrderingDomainException("Invalid units");
            }

            Units += units;
        }
    }
}
