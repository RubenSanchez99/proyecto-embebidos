using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Ordering.SignalrHub.IntegrationEvents;
using Ordering.SignalrHub.IntegrationEvents.EventHandling;
using eShopOnContainers.Services.IntegrationEvents.Events;
using Ordering.SignalrHub.Services;
using StackExchange.Redis;
using System;
using System.IdentityModel.Tokens.Jwt;
using MassTransit;
using MassTransit.Logging;
using System.Collections.Generic;

namespace Ordering.SignalrHub
{
    public class Startup
    {
        public Startup(IConfiguration configuration,  ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _loggerFactory = loggerFactory;
        }
        
        private ILoggerFactory _loggerFactory { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IHostedService, MassTransitHostedService>();
            services.AddScoped<OrderStatusChangedToAwaitingValidationIntegrationEventHandler>();
            services.AddScoped<OrderStatusChangedToCancelledIntegrationEventHandler>();
            services.AddScoped<OrderStatusChangedToPaidIntegrationEventHandler>();
            services.AddScoped<OrderStatusChangedToShippedIntegrationEventHandler>();
            services.AddScoped<OrderStatusChangedToStockConfirmedIntegrationEventHandler>();
            services.AddScoped<OrderStatusChangedToSubmittedIntegrationEventHandler>();
            services.AddScoped<AccountDepositIntegrationEventHandler>();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            if (Configuration.GetValue<string>("IsClusterEnv") == bool.TrueString)
            {
                services
                    .AddSignalR()
                    .AddRedis(Configuration["SignalrStoreConnectionString"]);
            }
            else
            {
                services.AddSignalR();
            }

            
            ConfigureAuthService(services);

            services.AddOptions();

            //configure autofac
            var container = new ContainerBuilder();

            container.Register(c =>
            {
                var busControl = Bus.Factory.CreateUsingRabbitMq(sbc => 
                {
                    var host = sbc.Host(new Uri(Configuration["EventBusConnection"]), h =>
                    {
                        h.Username(Configuration["EventBusUserName"]);
                        h.Password(Configuration["EventBusPassword"]);
                    });
                    sbc.ReceiveEndpoint(host, "order_awaitingvalidation_signalr_queue", e => 
                    {
                        e.Consumer<OrderStatusChangedToAwaitingValidationIntegrationEventHandler>(c);
                    });
                    sbc.ReceiveEndpoint(host, "order_cancelled_signalr_queue", e => 
                    {
                        e.Consumer<OrderStatusChangedToCancelledIntegrationEventHandler>(c);
                    });
                    sbc.ReceiveEndpoint(host, "order_paid_signalr_queue", e => 
                    {
                        e.Consumer<OrderStatusChangedToPaidIntegrationEventHandler>(c);
                    });
                    sbc.ReceiveEndpoint(host, "order_shipped_signalr_queue", e => 
                    {
                        e.Consumer<OrderStatusChangedToShippedIntegrationEventHandler>(c);
                    });
                    sbc.ReceiveEndpoint(host, "order_stockconfirmed_signalr_queue", e => 
                    {
                        e.Consumer<OrderStatusChangedToStockConfirmedIntegrationEventHandler>(c);
                    });
                    sbc.ReceiveEndpoint(host, "order_submitted_signalr_queue", e => 
                    {
                        e.Consumer<OrderStatusChangedToSubmittedIntegrationEventHandler>(c);
                    });
                    sbc.ReceiveEndpoint(host, "payment_amount_deposit", e => 
                    {
                        e.Consumer<AccountDepositIntegrationEventHandler>(c);
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

            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                loggerFactory.CreateLogger("init").LogDebug($"Using PATH BASE '{pathBase}'");
                app.UsePathBase(pathBase);
            }

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseSignalR(routes =>
            {
                routes.MapHub<NotificationsHub>("/notificationhub", options =>
                    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransports.All);
            });
        }

        private void ConfigureAuthService(IServiceCollection services)
        {
            // prevent from mapping "sub" claim to nameidentifier.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

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
                        "postman", "mvc"
                    }
                };

            });
        }
    }
}
