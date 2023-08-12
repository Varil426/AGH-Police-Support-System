using System.Reflection;
using Shared.Infrastructure;
using Shared.Simulation;

var host = Host.CreateDefaultBuilder(args)
    .UseAutofac()
    .AddSerilog()
    .AddSharedAppSettings()
    .AddRabbitMqSettings()
    .AddServiceSettings()
    .AddRabbitMqBus(new Assembly[] { }) // TODO
    .ConfigureServices(
        services =>
            services
                .AddMessageService()
                .AddMessageBus()
        // .AddHostedService<Worker>() // TODO Remove
        // .AddHostedService<HqAgent>(x => new HqAgent(/*Guid.NewGuid()*/Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e"), x.GetRequiredService<IMessageService>()))
        // TODO Create agents
    )
    .AddSharedSimulation(new Assembly[] { }) // TODO
    .Build();

host.SubscribeHandlers(new Assembly[] { }); // TODO
host.SubscribeSimulationMessageHandlers(new Assembly[] { }); // TODO

host.Run();