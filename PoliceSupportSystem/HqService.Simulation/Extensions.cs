using Autofac;
using Microsoft.Extensions.Hosting;

namespace HqService.Simulation;

public static class Extensions
{
    public static IHostBuilder AddSimulation(this IHostBuilder hostBuilder) =>
        hostBuilder.AddSimulatedServices()/*.AddSimulationMessageHandlers(new[] { typeof(Extensions).Assembly })*/;

    private static IHostBuilder AddSimulatedServices(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices(
            (ctx, s) => { });

        hostBuilder.ConfigureContainer<ContainerBuilder>(
            (ctx, builder) => { });

        return hostBuilder;
    }
}