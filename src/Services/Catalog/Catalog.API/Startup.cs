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

// Para EF Core
using Catalog.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection;

// Para MassTransit
using Catalog.API.Services;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.AutofacIntegration;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Catalog.API.IntegrationEvents.EventHandling;

namespace Catalog.API
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

            // Para EF Core
            services.AddEntityFrameworkNpgsql().AddDbContext<CatalogContext>(options =>
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

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    policyBuilder => policyBuilder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.Configure<CatalogSettings>(Configuration);

            // Configurar MassTransit
            services.AddScoped<IHostedService, MassTransitHostedService>();

            var builder = new ContainerBuilder(); 

            services.AddScoped<OrderStatusChangedToAwaitingValidationIntegrationEventHandler>();
            services.AddScoped<OrderStatusChangedToPaidIntegrationEventHandler>();

            Console.WriteLine(Configuration["EventBusConnection"]);

            builder.Register(c =>
            {
                var busControl = Bus.Factory.CreateUsingRabbitMq(sbc => 
                    {
                        var host = sbc.Host(new Uri(Configuration["EventBusConnection"]), h =>
                        {
                            h.Username(Configuration["EventBusUserName"]);
                            h.Password(Configuration["EventBusPassword"]);
                        });
                        sbc.ReceiveEndpoint(host, "catalog_validation_queue", e => 
                        {
                            e.Consumer<OrderStatusChangedToAwaitingValidationIntegrationEventHandler>(c);
                        });
                        sbc.ReceiveEndpoint(host, "catalog_orderpaid_queue", e => 
                        {
                            e.Consumer<OrderStatusChangedToPaidIntegrationEventHandler>(c);
                        });
                        sbc.UseExtensionsLogging(_loggerFactory);
                    }
                );
                var consumeObserver = new ConsumeObserver(_loggerFactory.CreateLogger<ConsumeObserver>());
                busControl.ConnectConsumeObserver(consumeObserver);

                var sendObserver = new SendObserver(_loggerFactory.CreateLogger<SendObserver>());
                busControl.ConnectSendObserver(sendObserver);

                return busControl;
            })
            .As<IBusControl>()
            .As<IPublishEndpoint>()
            .SingleInstance();
            builder.Populate(services);
            var container = builder.Build();

            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, CatalogContext db)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseCors("CorsPolicy");
        }
    }
}
