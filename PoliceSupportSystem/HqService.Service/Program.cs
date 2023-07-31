using HqService.Application.Agents;
using HqService.Application.Handlers;
using HqService.Service;
using Shared.Application.Agents;
using Shared.Simulation;
using Shared.Infrastructure;

var host = Host.CreateDefaultBuilder(args)
    .UseAutofac()
    .AddSerilog()
    .AddSharedAppSettings()
    .AddRabbitMqSettings()
    .AddServiceSettings()
    .AddRabbitMqBus(new[] { typeof(TestCommandHandler).Assembly })
    .ConfigureServices(
        services =>
            services
                .AddMessageService()
                .AddMessageBus()
                .AddHostedService<Worker>() // TODO Remove
                .AddHostedService<HqAgent>(x => new HqAgent(/*Guid.NewGuid()*/Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e"), x.GetRequiredService<IMessageService>()))
        // TODO Create agents
    )
    .AddSimulation()
    .Build();

host.SubscribeHandlers(new [] { typeof(TestEventHandler).Assembly });
host.SubscribeSimulationMessageHandler();

host.Run();