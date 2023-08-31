using Simulation.Application;
using Simulation.Infrastructure;
using Simulation.Shared.Communication;
using Simulation.Simulation;

var host = Host.CreateDefaultBuilder(args)
    .UseAutofac()
    .AddSettings()
    .AddRabbitMqBus()
    .AddNpgsql()
    .AddHandlers(typeof(ISimulation).Assembly)
    .AddSerilog()
    .AddSimulationServices()
    .ConfigureServices(
        x =>
        {
            x.AddHostedService<SimulationRunner>();
        })
    .Build();

host.SubscribeMessageSubscriber(typeof(ISimulationMessage).Assembly);

if (host.Services.GetRequiredService<SimulationSettings>() is { } simulationSettings && simulationSettings.StartDelay != default)
{
    host.Services.GetRequiredService<ILogger<Program>>().LogInformation("Simulation start delayed by {DELAY}", simulationSettings.StartDelay);
    await Task.Delay(simulationSettings.StartDelay);
}

host.Run();