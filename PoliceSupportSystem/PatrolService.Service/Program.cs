using PatrolService.Application;
using PatrolService.Simulation;
using Shared.Infrastructure;
using Shared.Simulation;

var host = Host.CreateDefaultBuilder(args)
    .UseAutofac()
    .AddSerilog()
    .AddSharedAppSettings()
    .AddRabbitMqSettings()
    .AddServiceSettings()
    .AddPatrolSettings()
    .AddRabbitMqBus(new[] { typeof(ApplicationModule).Assembly })
    .ConfigureServices(
        services =>
            services
                .AddMessageService()
                .AddMessageBus()
    )
    .AddServiceStatusNotifier()
    .RegisterSharedApplicationModule()
    .RegisterModule<ApplicationModule>()
    .RegisterModule<SimulationModule>()
    .AddSharedSimulation(new[] { typeof(SimulationModule).Assembly })
    .AddPatrolSpecificSimulationServices()
    .Build();

host.SubscribeHandlers(new[] { typeof(ApplicationModule).Assembly });
host.SubscribeSimulationMessageHandlers(new[] { typeof(SimulationModule).Assembly });

host.Run();