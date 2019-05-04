using MassTransit;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Catalog.API.Services
{
    public class ConsumeObserver: IConsumeObserver
    {    
        private readonly ILogger<ConsumeObserver> _log;

        public ConsumeObserver(ILogger<ConsumeObserver> log)
        {
            _log = log;
        }

        Task IConsumeObserver.PreConsume<T>(ConsumeContext<T> context)
        {
            // called before the consumer's Consume method is called
            _log.LogInformation($"{context.Message.GetType().Name}: {JsonConvert.SerializeObject(context.Message, Formatting.Indented)} - Attempt no. {context.GetRetryAttempt()}");
            return Task.CompletedTask;
        }

        Task IConsumeObserver.PostConsume<T>(ConsumeContext<T> context)
        {
            // called after the consumer's Consume method is called
            // if an exception was thrown, the ConsumeFault method is called instead
            return Task.CompletedTask;
        }

        Task IConsumeObserver.ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
        {
            // called if the consumer's Consume method throws an exception
            return Task.CompletedTask;
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
        {
            throw new NotImplementedException();
        }
    }

}