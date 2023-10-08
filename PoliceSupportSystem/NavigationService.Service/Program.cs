using System.Reflection;
using NavigationService.Application;
using NavigationService.Simulation;
using Shared.Infrastructure;
using Shared.Simulation;

var host = Host.CreateDefaultBuilder(args)
    .UseAutofac()
    .AddSerilog()
    .AddSharedAppSettings()
    .AddRabbitMqSettings()
    .AddServiceSettings()
    .AddPatrolSettings()
    .AddRabbitMqBus(new Assembly[] { }) // TODO
    .ConfigureServices(
        services =>
            services
                .AddMessageService()
                .AddMessageBus()
        // .AddHostedService<Worker>() // TODO Remove
        // .AddHostedService<HqAgent>(x => new HqAgent(/*Guid.NewGuid()*/Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e"), x.GetRequiredService<IMessageService>()))
    )
    .AddServiceStatusNotifier()
    .RegisterSharedApplicationModule()
    .RegisterModule<ApplicationModule>()
    .AddSharedSimulation(new[] { typeof(SimulationModule).Assembly })
    .RegisterModule<SimulationModule>()
    .AddPatrolSpecificSimulationServices()
    .Build();

host.SubscribeHandlers(new Assembly[] { }); // TODO
host.SubscribeSimulationMessageHandlers(new[] { typeof(SimulationModule).Assembly });

host.Run();