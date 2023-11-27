using HqService.Application;
using HqService.Infrastructure;
using HqService.Simulation;
using Shared.Simulation;
using Shared.Infrastructure;

var builder = Host.CreateDefaultBuilder(args);
var host = builder
    .UseAutofac()
    .AddSerilog()
    .AddSharedAppSettings()
    .AddRabbitMqSettings()
    .AddServiceSettings()
    .AddHqAgentSettings()
    .AddRabbitMqBus(new[] { typeof(ApplicationModule).Assembly })
    .ConfigureServices(
        services =>
            services
                .AddMessageService()
                .AddMessageBus()
    )
    .AddServiceStatusNotifier()
    .RegisterSharedApplicationModule()
    // .RegisterModule<ApplicationModule>()
    .ConfigureAppConfiguration(configurationBuilder => builder.RegisterModule(new ApplicationModule(configurationBuilder.Build())))
    .AddSharedSimulation(new[] { typeof(SimulationModule).Assembly })
    .RegisterModule<SimulationModule>()
    .Build();

host.SubscribeHandlers(new [] { typeof(ApplicationModule).Assembly });
host.SubscribeSimulationMessageHandlers(new[] { typeof(SimulationModule).Assembly });

host.Run();