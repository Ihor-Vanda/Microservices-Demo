using System.Reflection;
using Demo.Common.Settings;
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
                    if (configuration != null)
                    {
                        var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                        var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();

                        if (serviceSettings != null && rabbitMQSettings != null)
                        {

                            configurator.Host(rabbitMQSettings.Host);
                            configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
                            configurator.UseMessageRetry(retryConfigurator =>
                            {
                                retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
                            });
                        }
                    }
                });
            });

            return services;
        }
    }
}