using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Basket.API.Infrastructure.Repositories;
using Basket.API.IntegrationEvents.EventHandling;
using Basket.API.IntegrationEvents.Events;
using Basket.API.Model;
using EventBus;
using EventBus.Abstractions;
using EventBusRabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace Basket.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.Configure<BasketSettings>(Configuration);

            services.AddSingleton<ConnectionMultiplexer>(sp =>
            {
                //var settings = sp.GetRequiredService<IOptions<BasketSettings>>().Value;
                var configuration = ConfigurationOptions.Parse("localhost", true);

                configuration.ResolveDns = true;

                return ConnectionMultiplexer.Connect(configuration);
            });

            services.AddSingleton<IRabbitMqPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMqPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = "localhost",
                    UserName = "guest",
                    Password = "guest",
                    DispatchConsumersAsync = true
                };

                var retryCount = 5;

                return new DefaultRabbitMqPersistentConnection(factory, retryCount);
            });
            
            RegisterEventBus(services);

            services.AddTransient<IBasketRepository, RedisBasketRepository>();

            services.AddCors();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "My API", Version = "v1"});
            });

            var container = new ContainerBuilder();
            container.Populate(services);
            
            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseSwagger().UseSwaggerUI(s => { s.SwaggerEndpoint("/swagger/v1/swagger.json", "MySite"); });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
            });
            
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyHeader());
            
            ConfigureEventBus(app);
        }
        
         private void RegisterEventBus(IServiceCollection services)
         {
             var subscriptionClientName = "testQueue";


             services.AddSingleton<IEventBus, EventBusRabbitMq>(sp =>
             {
                 var rabbitMqPersistentConnection = sp.GetRequiredService<IRabbitMqPersistentConnection>();
                 var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                 var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                 var retryCount = 5;

                 return new EventBusRabbitMq(rabbitMqPersistentConnection, iLifetimeScope,
                     eventBusSubcriptionsManager, subscriptionClientName, retryCount);
             });

             services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

             services.AddTransient<ProductPriceChangedIntegrationEventHandler>();
             services.AddTransient<OrderStartedIntegrationEventHandler>();
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<ProductPriceChangedIntegrationEvent, ProductPriceChangedIntegrationEventHandler>();
            eventBus.Subscribe<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>();
        }
    }
}