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

host.Run();