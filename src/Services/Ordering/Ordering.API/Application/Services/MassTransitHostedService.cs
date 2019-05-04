using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using System;

namespace Ordering.API.Services
{
    public class MassTransitHostedService : Microsoft.Extensions.Hosting.IHostedService
    {
        private readonly IBusControl busControl;

        public MassTransitHostedService(IBusControl busControl)
        {
            this.busControl = busControl;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //start the bus
            await busControl.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            //stop the bus
            await busControl.StopAsync(TimeSpan.FromSeconds(10));
        }
    }
}