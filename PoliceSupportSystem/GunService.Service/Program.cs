using GunService.Application;
using GunService.Simulation;
using Shared.Infrastructure;
using Shared.Simulation;

var applicationAssemblies = new[] { typeof(ApplicationModule).Assembly };
var simulationAssemblies = new[] { typeof(SimulationModule).Assembly };

var host = Host.CreateDefaultBuilder(args)
    .UseAutofac()
    .AddSerilog()
    .AddSharedAppSettings()
    .AddRabbitMqSettings()
    .AddServiceSettings()
    .AddPatrolSettings()
    .AddRabbitMqBus(applicationAssemblies)
    .ConfigureServices(
        services =>
            services
                .AddMessageService()
                .AddMessageBus()
        // TODO Create agents
    )
    .AddServiceStatusNotifier()
    .RegisterSharedApplicationModule()
    .AddSharedSimulation(simulationAssemblies)
    .AddPatrolSpecificSimulationServices()
    .Build();

host.SubscribeHandlers(applicationAssemblies);
host.SubscribeSimulationMessageHandlers(simulationAssemblies);

host.Run();