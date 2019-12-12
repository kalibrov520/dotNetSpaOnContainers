using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Catalog.API.Infrastructure;
using Catalog.API.IntegrationEvents;
using EventBus;
using EventBus.Abstractions;
using EventBusRabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;

namespace Catalog.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc().Services
                .AddCustomMvc(Configuration)
                .AddCustomDbContext(Configuration)
                .AddCustomOptions(Configuration)
                .AddIntegrationServices(Configuration)
                .AddEventBus(Configuration)
                .AddCors()
                .AddSwagger(Configuration);

            var container = new ContainerBuilder();
            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseSwagger().UseSwaggerUI(s => { s.SwaggerEndpoint("/swagger/v1/swagger.json", "MySite"); });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
                /*endpoints.MapGet("/_proto/", async ctx =>
                {
                    ctx.Response.ContentType = "text/plain";
                    using var fs = new FileStream(Path.Combine(env.ContentRootPath, "Proto", "catalog.proto"), FileMode.Open, FileAccess.Read);
                    using var sr = new StreamReader(fs);
                    while (!sr.EndOfStream)
                    {
                        var line = await sr.ReadLineAsync();
                        if (line != "/* >>" || line != "<< #1#")
                        {
                            await ctx.Response.WriteAsync(line);
                        }
                    }
                });
                endpoints.MapGrpcService<CatalogService>();*/
            });
            
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyHeader());
        }
    }

    public static class CustomExtensionMethods
    {
        public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            return services;
        }

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<CatalogContext>(options => options.UseSqlServer("Data Source=IKALIBROV\\SQLEXPRESS;Initial Catalog=test;Integrated Security=True"));

            return services;
        }

        public static IServiceCollection AddCustomOptions(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<CatalogSettings>(configuration);

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "My API", Version = "v1"});
            });

            return services;
        }
        
         public static IServiceCollection AddIntegrationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ICatalogIntegrationEventService, CatalogIntegrationEventService>();

            services.AddSingleton<IRabbitMqPersistentConnection>(sp =>
            {
                var factory = new ConnectionFactory()
                {
                    HostName = /*configuration["EventBusConnection"],*/"testHost",
                    DispatchConsumersAsync = true
                };

//                if (!string.IsNullOrEmpty(configuration["EventBusUserName"]))
//                {
//                    factory.UserName = configuration["EventBusUserName"];
//                }
//
//                if (!string.IsNullOrEmpty(configuration["EventBusPassword"]))
//                {
//                    factory.Password = configuration["EventBusPassword"];
//                }

                var retryCount = 5;
//                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
//                {
//                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
//                }

                return new DefaultRabbitMqPersistentConnection(factory, retryCount);
            });
            
            return services;
        }
         
          public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
          {
              var subscriptionClientName = /*configuration["SubscriptionClientName"];*/"testClient";


              services.AddSingleton<IEventBus, EventBusRabbitMq>(sp =>
              {
                  var rabbitMqPersistentConnection = sp.GetRequiredService<IRabbitMqPersistentConnection>();
                  var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                  var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                  var retryCount = 5;
                  if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                  {
                      retryCount = int.Parse(configuration["EventBusRetryCount"]);
                  }

                  return new EventBusRabbitMq(rabbitMqPersistentConnection, iLifetimeScope,
                      eventBusSubcriptionsManager,
                      subscriptionClientName, retryCount);
              });

              services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

              /*services.AddTransient<OrderStatusChangedToAwaitingValidationIntegrationEventHandler>();
              services.AddTransient<OrderStatusChangedToPaidIntegrationEventHandler>();*/

              return services;
          }
    }
}