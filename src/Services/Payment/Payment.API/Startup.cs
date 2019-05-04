using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Payment.API.IntegrationEvents.EventHandling;
using eShopOnContainers.Services.IntegrationEvents.Events;
using System;
using System.Threading.Tasks;
using MassTransit;
using Payment.API.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Payment.API.Infrastructure;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Payment.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        public IConfiguration Configuration { get; }
        private ILoggerFactory _loggerFactory { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            services.Configure<PaymentSettings>(Configuration);

            services.AddEntityFrameworkNpgsql().AddDbContext<PaymentContext>(options =>
            {
                options.UseNpgsql(Configuration["ConnectionString"],
                                     npgsqlOptionsAction: sqlOptions =>
                                     {
                                         sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                                         //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                         sqlOptions.EnableRetryOnFailure(maxRetryCount: 5);
                                     });

                // Changing default behavior when client evaluation occurs to throw. 
                // Default in EF Core would be to log a warning when client evaluation is performed.
                options.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));
                //Check Client vs. Server evaluation: https://docs.microsoft.com/en-us/ef/core/querying/client-eval
            });

            var containerBuilder = new ContainerBuilder();

            services.AddScoped<IHostedService, MassTransitHostedService>();
            services.AddScoped<OrderStatusChangedToStockConfirmedIntegrationEventHandler>();

            containerBuilder.Register(c =>
            {
                var busControl = Bus.Factory.CreateUsingRabbitMq(sbc => 
                {
                    var host = sbc.Host(new Uri(Configuration["EventBusConnection"]), h =>
                    {
                        h.Username(Configuration["EventBusUserName"]);
                        h.Password(Configuration["EventBusPassword"]);
                    });
                    sbc.ReceiveEndpoint(host, "validate_payment_queue", e => 
                    {
                        e.Consumer<OrderStatusChangedToStockConfirmedIntegrationEventHandler>(c);
                    });
                    sbc.UseExtensionsLogging(_loggerFactory);
                });
                var consumeObserver = new ConsumeObserver(_loggerFactory.CreateLogger<ConsumeObserver>());
                busControl.ConnectConsumeObserver(consumeObserver);

                var sendObserver = new SendObserver(_loggerFactory.CreateLogger<SendObserver>());
                busControl.ConnectSendObserver(sendObserver);

                return busControl;
            })
            .As<IBusControl>()
            .As<IPublishEndpoint>()
            .SingleInstance();


            containerBuilder.Populate(services);
            return new AutofacServiceProvider(containerBuilder.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }
            app.UseMvc();
        }
    }
}