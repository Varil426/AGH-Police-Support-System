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
                // .AddHostedService<Worker>() // TODO Remove
                // .AddHostedService<HqAgent>(x => new HqAgent(/*Guid.NewGuid()*/Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e"), x.GetRequiredService<IMessageService>()))
        // TODO Create agents
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