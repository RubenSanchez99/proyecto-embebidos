using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EventFlow.Aggregates;
using EventFlow.ReadStores;
using Ordering.Domain.Events;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
using Ordering.Domain.AggregatesModel.OrderAggregate;

namespace Ordering.ReadModel.Model
{
    public class OrderReadModel : IReadModel,
        IAmReadModelFor<Order, OrderId, OrderStartedDomainEvent>,
        IAmReadModelFor<Order, OrderId, OrderStatusChangedToAwaitingValidationDomainEvent>,
        IAmReadModelFor<Order, OrderId, OrderStatusChangedToStockConfirmedDomainEvent>,
        IAmReadModelFor<Order, OrderId, OrderStatusChangedToPaidDomainEvent>,
        IAmReadModelFor<Order, OrderId, OrderShippedDomainEvent>,
        IAmReadModelFor<Order, OrderId, OrderCancelledDomainEvent>,
        IAmReadModelFor<Order, OrderId, OrderBuyerChangedDomainEvent>
    {
        [Key]
        public string OrderId { get; set; }
        public int OrderNumber { get; set; }
        public string BuyerIdentityGuid { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public List<OrderItemReadModel> OrderItems { get; set; }
        public decimal Total { get; set; }

        public void Apply(IReadModelContext context, IDomainEvent<Order, OrderId, OrderStartedDomainEvent> domainEvent)
        {
            this.OrderId = domainEvent.AggregateIdentity.Value;
            this.Date = domainEvent.AggregateEvent.OrderDate;
            this.Status = OrderStatus.Submitted.Name;
            this.Description = "";
            this.Street = domainEvent.AggregateEvent.Address.Street;
            this.City = domainEvent.AggregateEvent.Address.City;
            this.ZipCode = domainEvent.AggregateEvent.Address.ZipCode;
            this.Country = domainEvent.AggregateEvent.Address.Country;
            this.OrderItems = new List<OrderItemReadModel>();
            this.Total = 0;
            foreach (var item in domainEvent.AggregateEvent.Items)
            {
                this.OrderItems.Add( new OrderItemReadModel{
                    ProductName = item.ProductName,
                    Units = item.Units,
                    UnitPrice = item.UnitPrice,
                    PictureUrl = item.PictureUrl
                });
                this.Total += item.Units * item.UnitPrice;
            }
        }

        public void Apply(IReadModelContext context, IDomainEvent<Order, OrderId, OrderStatusChangedToAwaitingValidationDomainEvent> domainEvent)
        {
            this.Status = OrderStatus.AwaitingValidation.Name;
        }

        public void Apply(IReadModelContext context, IDomainEvent<Order, OrderId, OrderStatusChangedToStockConfirmedDomainEvent> domainEvent)
        {
            this.Status = OrderStatus.StockConfirmed.Name;
        }

        public void Apply(IReadModelContext context, IDomainEvent<Order, OrderId, OrderStatusChangedToPaidDomainEvent> domainEvent)
        {
            this.Status = OrderStatus.Shipped.Name;
        }

        public void Apply(IReadModelContext context, IDomainEvent<Order, OrderId, OrderShippedDomainEvent> domainEvent)
        {
            this.Status = OrderStatus.Shipped.Name;
        }

        public void Apply(IReadModelContext context, IDomainEvent<Order, OrderId, OrderCancelledDomainEvent> domainEvent)
        {
            this.Status = OrderStatus.Cancelled.Name;
            this.Description = domainEvent.AggregateEvent.Description;
        }

        public void Apply(IReadModelContext context, IDomainEvent<Order, OrderId, OrderBuyerChangedDomainEvent> domainEvent)
        {
            this.BuyerIdentityGuid = domainEvent.AggregateEvent.BuyerId.Value;
        }
    }
}
