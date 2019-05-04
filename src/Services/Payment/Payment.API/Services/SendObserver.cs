using MassTransit;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Payment.API.Services
{
    public class SendObserver : ISendObserver
    {
        private readonly ILogger<SendObserver> _log;

        public SendObserver(ILogger<SendObserver> log)
        {
            _log = log;
        }

        public Task PostSend<T>(SendContext<T> context) where T : class
        {
            _log.LogInformation($"{context.Message.GetType().Name}: {JsonConvert.SerializeObject(context.Message, Formatting.Indented)}");
            return Task.CompletedTask;
        }

        public Task PreSend<T>(SendContext<T> context) where T : class
        {
            return Task.CompletedTask;
        }

        public Task SendFault<T>(SendContext<T> context, Exception exception) where T : class
        {
            return Task.CompletedTask;
        }
    }
}