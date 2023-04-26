using System.Reflection;
using Demo.Common.Settings;
using Demo.RabbitMQ;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Catalog.MassTransit
{
    public static class Extentions
    {
        public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
        {
            services.AddMassTransit(configure =>
            {
                configure.AddConsumers(Assembly.GetEntryAssembly());

                configure.UsingRabbitMq((context, configurator) =>
               {
                   var configuration = context.GetService<IConfiguration>();
                   var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                   var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();

                   configurator.Host(rabbitMQSettings.Host);
                   configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
               });
            });

            return services;
        }
    }
}