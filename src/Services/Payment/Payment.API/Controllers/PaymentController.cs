using eShopOnContainers.Services.IntegrationEvents.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Payment.API.Infrastructure;
using Payment.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payment.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentContext _context;
        private readonly IPublishEndpoint _endpoint;
        public PaymentController(PaymentContext context, IPublishEndpoint endpoint)
        {
            _context = context;
            _endpoint = endpoint;
        }

        [HttpGet("{buyerId}")]
        public ActionResult<decimal> Get(string buyerId)
        {
            Guid guid = new Guid(buyerId);

            return _context.Accounts.Single(x => x.BuyerId == guid).AmountAvailable;
        }

        [HttpPost("{buyerID}")]
        public void Post(string buyerId, [FromBody] decimal amount)
        {
            Guid guid = new Guid(buyerId);

            var userAccount = _context.Accounts.Single(x => x.BuyerId == guid);
            userAccount.AmountAvailable += amount;
            _context.SaveChanges();

            _endpoint.Publish(new AccountDepositIntegrationEvent("bob", amount, userAccount.AmountAvailable));
        }
    }
}