using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Application.Commands;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using EventFlow;
using EventFlow.Queries;
using Ordering.API.Infrastructure.Services;
using Ordering.ReadModel.Queries;
using Ordering.ReadModel.Model;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate.Identity;
using System.Threading;
using EventFlow.Aggregates.ExecutionResults;
using Ordering.API.Application.Models;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class OrdersController : Controller
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IIdentityService _identityService;

        public OrdersController(ICommandBus commandBus, IQueryProcessor queryProcessor, IIdentityService identityService)
        {
            _commandBus = commandBus ?? throw new ArgumentNullException(nameof(commandBus));
            _queryProcessor = queryProcessor ?? throw new ArgumentNullException(nameof(queryProcessor));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        [Route("cancel")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CancelOrder([FromBody]CancelOrderCommand command, [FromHeader(Name = "x-requestid")] string requestId)
        {
            IExecutionResult result = ExecutionResult.Failed();
            if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
            {
                var order = await _queryProcessor.ProcessAsync(new GetOrderByOrderNumberQuery(command.OrderNumber), CancellationToken.None).ConfigureAwait(false);
                await _commandBus.PublishAsync(new CancelOrderCommand(new OrderId(order.OrderId), order.OrderNumber), CancellationToken.None);
            }
           
            return result.IsSuccess ? (IActionResult)Ok() : (IActionResult)BadRequest();

        }

        [Route("ship")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult ShipOrder([FromBody]ShipOrderCommand command, [FromHeader(Name = "x-requestid")] string requestId)
        {
            return NotFound();

        }

        [Route("{orderId}")]
        [HttpGet]
        [ProducesResponseType(typeof(OrderReadModel),(int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetOrder(string orderId)
        {
            try
            {
                var order = await _queryProcessor.ProcessAsync(new GetOrderQuery(orderId), CancellationToken.None).ConfigureAwait(false);
                return Ok(order);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderSummaryReadModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrders()
        {
            var userid = _identityService.GetUserIdentity();
            var orders = await _queryProcessor.ProcessAsync(new GetOrdersFromUserQuery(Guid.Parse(userid)), CancellationToken.None).ConfigureAwait(false);
            return Ok(orders);
        }

        [Route("cardtypes")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CardType>), (int)HttpStatusCode.OK)]
        public IActionResult GetCardTypes()
        {
            return NotFound();
        }        

        [Route("draft")]
        [HttpPost]
        public async Task<IActionResult> GetOrderDraftFromBasketData([FromBody] CustomerBasket basket)
        {
            var result = await _commandBus.PublishAsync(new CreateOrderDraftCommand(OrderId.New, basket.BuyerId, basket.Items), CancellationToken.None);
            
           return result.IsSuccess ? (IActionResult)Ok(result.Order) : (IActionResult)BadRequest();
        }
    }
}


