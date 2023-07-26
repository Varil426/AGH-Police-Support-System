using Simulation.Application;
using Simulation.Infrastructure;
using Simulation.Simulation;

var host = Host.CreateDefaultBuilder(args)
    .UseAutofac()
    .AddSettings()
    .AddRabbitMqBus()
    .AddHandlers(typeof(ISimulation).Assembly)
    .AddMessageSubscriber()
    // TODO Add logging
    .ConfigureServices(
        x =>
        {
            
            x.AddSingleton<ISimulation, Simulation.Application.Simulation>();
            x.AddHostedService<SimulationRunner>();
        })
    .Build();

host.SubscribeMessageSubscriber(typeof(ISimulation).Assembly);

host.Run();