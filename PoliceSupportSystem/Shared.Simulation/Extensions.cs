using Autofac;
using Autofac.Features.AttributeFilters;
using MessageBus.Core;
using MessageBus.Core.API;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Simulation.Services;
using Shared.Simulation.Settings;

namespace Shared.Simulation;

public static class Extensions
{
    public static IHostBuilder AddSimulation(this IHostBuilder hostBuilder) => hostBuilder
        .AddRabbitMqSimulationBus()
        .AddInternalServices();

    private static IHostBuilder AddRabbitMqSimulationBus(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureContainer<ContainerBuilder>(
            (ctx, builder) =>
            {
                var simulationCommunicationSettings = ctx.Configuration.GetSettings<SimulationCommunicationSettings>(nameof(SimulationCommunicationSettings));

                builder.Register(
                    _ => new RabbitMQBus(
                        configurator =>
                        {
                            configurator.UseConnectionString(
                                $"amqp://{simulationCommunicationSettings.Username}:{simulationCommunicationSettings.Password}@{simulationCommunicationSettings.Host}:{simulationCommunicationSettings.Port}/");
                        })).As<IBus>().Keyed<IBus>(Constants.SimulationBusKey).SingleInstance();
                
                // builder.Register(
                //     _ => new RabbitMQBus(
                //         configurator =>
                //         {
                //             configurator.UseConnectionString(
                //                 $"amqp://{simulationCommunicationSettings.Username}:{simulationCommunicationSettings.Password}@{simulationCommunicationSettings.Host}:{simulationCommunicationSettings.Port}/");
                //         })).As<IBus>().Keyed<IBus>(Constants.SimulationBusKey);
            });

        return hostBuilder;
    }
    
    private static IHostBuilder AddInternalServices(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices(
            (ctx, s) =>
            {
                s.AddSingleton(ctx.Configuration.GetSettings<SimulationCommunicationSettings>(nameof(SimulationCommunicationSettings)));
            });
        
        hostBuilder.ConfigureContainer<ContainerBuilder>(
            (ctx, builder) =>
            {
                builder.RegisterType<SimulationMessageBus>().As<ISimulationMessageBus>().WithAttributeFiltering();
            });

        return hostBuilder;
    }
    
    private static TSettings GetSettings<TSettings>(this IConfiguration configuration, string sectionName)
    {
        var configSection = configuration.GetRequiredSection(sectionName);
        var settings = configSection.Get<TSettings>() ?? throw new Exception(); // TODO Change exception type
        return settings;
    }
}