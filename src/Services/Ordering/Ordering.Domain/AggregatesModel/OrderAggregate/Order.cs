using System;
using System.Collections.Generic;
using System.Linq;
using EventFlow.Aggregates;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Core;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.BuyerAggregate.Identity;
using Ordering.Domain.Events;
using Ordering.Domain.Exceptions;
using Ordering.Domain.ExecutionResults;
using Newtonsoft.Json;

namespace Ordering.Domain.AggregatesModel.OrderAggregate
{
    public class Order : AggregateRoot<Order, OrderId>,
    IEmit<OrderStartedDomainEvent>,
    IEmit<OrderPaymentMethodChangedDomainEvent>,
    IEmit<OrderBuyerChangedDomainEvent>,
    IEmit<OrderStatusChangedToAwaitingValidationDomainEvent>,
    IEmit<OrderStatusChangedToStockConfirmedDomainEvent>,
    IEmit<OrderStatusChangedToPaidDomainEvent>,
    IEmit<OrderShippedDomainEvent>,
    IEmit<OrderCancelledDomainEvent>
    {

        // DDD Patterns comment
        // Using private fields, allowed since EF Core 1.1, is a much better encapsulation
        // aligned with DDD Aggregates and Domain Entities (Instead of properties and property collections)
        private DateTime _orderDate;

        // Address is a Value Object pattern example persisted as EF Core 2.0 owned entity
        public Address Address { get; private set; }

        public BuyerId GetBuyerId => _buyerId;
        private BuyerId _buyerId;

        public OrderStatus OrderStatus => OrderStatus.FromValue<OrderStatus>(_orderStatusId);
        private int _orderStatusId;

        private string _description;


        // Draft orders have this set to true. Currently we don't check anywhere the draft status of an Order, but we could do it if needed
        private bool _isDraft;

        // DDD Patterns comment
        // Using a private collection field, better for DDD Aggregate's encapsulation
        // so OrderItems cannot be added from "outside the AggregateRoot" directly to the collection,
        // but only through the method OrderAggrergateRoot.AddOrderItem() which includes behaviour.
        private List<OrderItem> _orderItems;
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

        private PaymentMethodId _paymentMethodId;


        
        public OrderDraftExcecutionResult NewDraft(string buyerId, IEnumerable<OrderItem> items)
        {
            var itemList = CreateItemList(items);
            var order = new OrderDraft()
            {
                OrderNumber = 0,
                Date = DateTime.Now,
                Status = OrderStatus.Submitted.Name,
                Description = "Order draft for user " + buyerId,
                Street = "",
                City = "",
                ZipCode = "",
                Country = "",
                OrderItems = itemList.Select(x => new OrderDraftItem(x.ProductName, x.UnitPrice, x.Units, x.PictureUrl)).ToList(),
                Total = itemList.Sum(x => x.UnitPrice * x.Units)
            };
            return new OrderDraftExcecutionResult(true, order);
        }

        public Order(OrderId id) : base(id)
        {
            _orderItems = new List<OrderItem>();
            _isDraft = false;
        }

        private List<OrderItem> CreateItemList(IEnumerable<OrderItem> items)
        {
            var itemList = new List<OrderItem>();
            foreach (var item in items)
            {
                var existingOrderForProduct = itemList.Where(o => o.ProductId == item.ProductId)
                .SingleOrDefault();

                if (existingOrderForProduct != null)
                {
                    //if previous line exist modify it with higher discount  and units..

                    if (item.Discount > existingOrderForProduct.Discount)
                    {
                        existingOrderForProduct.SetNewDiscount(item.Discount);
                    }

                    existingOrderForProduct.AddUnits(item.Units);
                }
                else
                {
                    //add validated new order item
                    itemList.Add(item);
                }
            }
            return itemList;
        }

        public IExecutionResult Create(string userId, string userName, Address address, int cardTypeId, string cardNumber, string cardSecurityNumber,
                string cardHolderName, DateTime cardExpiration, IEnumerable<OrderItem> items, string buyerId = null)
        {
            var itemList = CreateItemList(items);

            var orderStarted = new OrderStartedDomainEvent(DateTime.UtcNow, address, userId, userName, cardTypeId, cardNumber, cardSecurityNumber, cardHolderName, cardExpiration, itemList);
            Emit(orderStarted);
            return ExecutionResult.Success();
        }

        public void Apply(OrderStartedDomainEvent aggregateEvent)
        {
            //_buyerId = aggregateEvent._buyerId;
            //_paymentMethodId = aggregateEvent.PaymentMethodId;
            _orderStatusId = OrderStatus.Submitted.Id;
            _orderDate = aggregateEvent.OrderDate;
            Address = aggregateEvent.Address;
            _orderItems = aggregateEvent.Items.ToList();
        }

        public void SetPaymentId(PaymentMethodId id)
        {
            Emit(new OrderPaymentMethodChangedDomainEvent(id));
        }

        public void SetBuyerId(BuyerId buyerId)
        {
            Emit(new OrderBuyerChangedDomainEvent(buyerId));
        }

        public void Apply(OrderBuyerChangedDomainEvent aggregateEvent)
        {
            _buyerId = aggregateEvent.BuyerId;
        }

        public void Apply(OrderPaymentMethodChangedDomainEvent aggregateEvent)
        {
            _paymentMethodId = aggregateEvent.PaymentMethodId;
        }

        public void SetAwaitingValidationStatus()
        {
            if (_orderStatusId == OrderStatus.Submitted.Id)
            {
                Emit(new OrderStatusChangedToAwaitingValidationDomainEvent(_orderItems));
            }
        }

        public void Apply(OrderStatusChangedToAwaitingValidationDomainEvent aggregateEvent)
        {
            _orderStatusId = OrderStatus.AwaitingValidation.Id;
        }

        public void SetStockConfirmedStatus()
        {
            if (_orderStatusId == OrderStatus.AwaitingValidation.Id)
            {
                Emit(new OrderStatusChangedToStockConfirmedDomainEvent());
            }
        }

        public void Apply(OrderStatusChangedToStockConfirmedDomainEvent aggregateEvent)
        {
            _orderStatusId = OrderStatus.StockConfirmed.Id;
            _description = "All the items were confirmed with available stock.";
        }

        public void SetPaidStatus()
        {
            if (_orderStatusId == OrderStatus.StockConfirmed.Id)
            {
                Emit(new OrderStatusChangedToPaidDomainEvent(OrderItems));                
            }
        }

        public void Apply(OrderStatusChangedToPaidDomainEvent aggregateEvent)
        {
            _orderStatusId = OrderStatus.Paid.Id;
            _description = "The payment was performed at a simulated \"American Bank checking bank account endinf on XX35071\"";
        }

        public IExecutionResult SetShippedStatus()
        {
            if (_orderStatusId != OrderStatus.Paid.Id)
            {
                StatusChangeException(OrderStatus.Shipped);
            }

            Emit(new OrderShippedDomainEvent());

            return ExecutionResult.Success();
        }

        public void Apply(OrderShippedDomainEvent aggregateEvent)
        {
            _orderStatusId = OrderStatus.Shipped.Id;
            _description = "The order was shipped.";
        }

        public IExecutionResult SetCancelledStatus()
        {
            if (_orderStatusId == OrderStatus.Paid.Id ||
                _orderStatusId == OrderStatus.Shipped.Id)
            {
                StatusChangeException(OrderStatus.Cancelled);
            }

            Emit(new OrderCancelledDomainEvent($"The order was cancelled."));
            return ExecutionResult.Success();
        }

        public void Apply(OrderCancelledDomainEvent aggregateEvent)
        {
            _orderStatusId = OrderStatus.Cancelled.Id;
            _description = aggregateEvent.Description;
        }

        public void SetCancelledStatusWhenStockIsRejected(IEnumerable<int> orderStockRejectedItems)
        {
            if (_orderStatusId == OrderStatus.AwaitingValidation.Id)
            {
                var itemsStockRejectedProductNames = OrderItems
                    .Where(c => orderStockRejectedItems.Contains(c.ProductId))
                    .Select(c => c.ProductName);

                var itemsStockRejectedDescription = string.Join(", ", itemsStockRejectedProductNames);
                var description = $"The product items don't have stock: ({itemsStockRejectedDescription}).";

                Emit(new OrderCancelledDomainEvent(description));
            }
        }

        private void StatusChangeException(OrderStatus orderStatusToChange)
        {
            throw new OrderingDomainException($"Is not possible to change the order status from {OrderStatus.Name} to {orderStatusToChange.Name}.");
        }

        public decimal GetTotal()
        {
            return _orderItems.Sum(o => o.Units * o.UnitPrice);
        }
    }
}
