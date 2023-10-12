using HqService.Application;
using HqService.Application.Handlers;
using HqService.Infrastructure;
using HqService.Simulation;
using Shared.Simulation;
using Shared.Infrastructure;

var host = Host.CreateDefaultBuilder(args)
    .UseAutofac()
    .AddSerilog()
    .AddSharedAppSettings()
    .AddRabbitMqSettings()
    .AddServiceSettings()
    .AddHqAgentSettings()
    .AddRabbitMqBus(new[] { typeof(TestCommandHandler).Assembly })
    .ConfigureServices(
        services =>
            services
                .AddMessageService()
                .AddMessageBus()
        // TODO Create agents
    )
    .AddServiceStatusNotifier()
    .RegisterSharedApplicationModule()
    .RegisterModule<ApplicationModule>()
    .AddSharedSimulation(new[] { typeof(SimulationModule).Assembly })
    .RegisterModule<SimulationModule>()
    .Build();

host.SubscribeHandlers(new [] { typeof(TestEventHandler).Assembly });
host.SubscribeSimulationMessageHandlers(new[] { typeof(SimulationModule).Assembly });

host.Run();