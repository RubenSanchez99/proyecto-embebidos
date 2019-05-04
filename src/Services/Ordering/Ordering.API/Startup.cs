using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventFlow;
using EventFlow.Extensions;
using EventFlow.Autofac.Extensions;
using EventFlow.Configuration;
using EventFlow.MetadataProviders;
using EventFlow.AspNetCore.MetadataProviders;
using EventFlow.AspNetCore.Extensions;
using EventFlow.Aspnetcore.Middlewares;
using EventFlow.EventStores.EventStore;
using EventFlow.EventStores.EventStore.Extensions;
using Ordering.API.Services;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.EntityFrameworkCoreIntegration.Saga;
using Microsoft.Extensions.Hosting;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Ordering.ReadModel;
using Ordering.API.Application.IntegrationEvents.EventHandling;
using Ordering.Domain.Events;
using Ordering.API.Application.Sagas;
using Microsoft.EntityFrameworkCore;
using Ordering.API.Infrastructure;
using GreenPipes;
using MassTransit.Saga;
using Ordering.API.Application.Subscribers;
using Ordering.API.Application.Commands;
using Ordering.API.Infrastructure.Services;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using EventFlow.Aggregates;

namespace Ordering.API
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
            var containerBuilder = new ContainerBuilder();

            services.AddEntityFrameworkNpgsql().AddDbContext<OrderingDbContext>(options =>
            {
                options.UseNpgsql(Configuration["ConnectionString"],
                                     npgsqlOptionsAction: sqlOptions =>
                                     {
                                         sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                                         //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                         sqlOptions.EnableRetryOnFailure(maxRetryCount: 5);
                                     });
            });
           
            ConfigureEventFlow(services, containerBuilder);

            ConfigureMassTransit(services, containerBuilder);            

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIdentityService, IdentityService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            ConfigureAuthService(services);

            containerBuilder.Populate(services);

            return new AutofacServiceProvider(containerBuilder.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            ConfigureAuth(app);

            app.UseHttpsRedirection();
            app.UseMiddleware<CommandPublishMiddleware>();
            app.UseMvc();
        }

        private void ConfigureAuthService(IServiceCollection services)
        {
            // prevent from mapping "sub" claim to nameidentifier.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var identityUrl = Configuration.GetValue<string>("IdentityUrl");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = "http://identity.api";
                options.MetadataAddress = Configuration["IdentityUrlExternal"] + "/.well-known/openid-configuration";
                options.RequireHttpsMetadata = false; 
                options.Audience = "http://identity.api/resources";
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidAudiences = new List<string> 
                    {
                        "postman",
                    }
                };

            });
        }

        private void ConfigureEventFlow(IServiceCollection services, ContainerBuilder containerBuilder)
        {
            var events = new List<Type>() {
                typeof(OrderStartedDomainEvent),
                typeof(BuyerCreatedDomainEvent),
                typeof(BuyerPaymentMethodAddedDomainEvent),
                typeof(BuyerAndPaymentMethodVerifiedDomainEvent),
                typeof(OrderStatusChangedToAwaitingValidationDomainEvent),
                typeof(OrderStatusChangedToPaidDomainEvent),
                typeof(OrderStatusChangedToStockConfirmedDomainEvent),
                typeof(OrderBuyerChangedDomainEvent),
                typeof(OrderPaymentMethodChangedDomainEvent),
                typeof(OrderCancelledDomainEvent),
                typeof(OrderShippedDomainEvent)
            };

            var commandHandlers = new List<Type> {
                typeof(CreateOrderCommandHandler),
                typeof(CancelOrderCommandHandler),
                typeof(ShipOrderCommandHandler),
                typeof(CreateOrderDraftCommandHandler)
            };

            var container = EventFlowOptions.New
                .UseAutofacContainerBuilder(containerBuilder)
                .UseConsoleLog()
                .AddEvents(events)
                .AddCommandHandlers(commandHandlers)
                .AddSubscribers(typeof(Startup).Assembly)
                .AddQueryHandlers(typeof(OrderingDbContext).Assembly)
                .AddEntityFrameworkReadModel();
        }

        private void ConfigureMassTransit(IServiceCollection services, ContainerBuilder containerBuilder)
        {
            services.AddScoped<IHostedService, MassTransitHostedService>();
            services.AddScoped<UserCheckoutAcceptedIntegrationEventHandler>();

            containerBuilder.Register(c =>
            {
                var busControl = Bus.Factory.CreateUsingRabbitMq(sbc => 
                {
                    var host = sbc.Host(new Uri(Configuration["EventBusConnection"]), h =>
                    {
                        h.Username(Configuration["EventBusUserName"]);
                        h.Password(Configuration["EventBusPassword"]);
                    });
                    sbc.ReceiveEndpoint(host, "basket_checkout_queue", e => 
                    {
                        e.Consumer<UserCheckoutAcceptedIntegrationEventHandler>(c);
                    });
                    sbc.ReceiveEndpoint(host, "stock_confirmed_queue", e => 
                    {
                    });
                    sbc.ReceiveEndpoint(host, "stock_rejected_queue", e => 
                    {
                    });
                    sbc.ReceiveEndpoint(host, "payment_succeded_queue", e => 
                    {
                    });
                    sbc.ReceiveEndpoint(host, "payment_failed_queue", e => 
                    {
                    });
                    sbc.ReceiveEndpoint(host, "graceperiod_confirmed_queue", e => 
                    {
                    });
                    sbc.ReceiveEndpoint(host, "order_validation_state", e =>
                    {
                        e.UseRetry(x => 
                            {
                                x.Handle<DbUpdateConcurrencyException>();
                                x.Interval(5, TimeSpan.FromMilliseconds(100));
                            }); // Add the retry middleware for optimistic concurrency
                        e.StateMachineSaga(new GracePeriodStateMachine(c.Resolve<IAggregateStore>()), new InMemorySagaRepository<GracePeriod>());
                    });
                    sbc.UseExtensionsLogging(_loggerFactory);
                    sbc.UseInMemoryScheduler();
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
        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            app.UseAuthentication();
        }
    }
}
