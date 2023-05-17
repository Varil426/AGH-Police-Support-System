using HqService.Application.Handlers;
using HqService.Service;
using Newtonsoft.Json;
using Shared.Infrastructure;
using Shared.Infrastructure.Exceptions;
using Shared.Application.Integration.Queries;
using Shared.Infrastructure.Settings;
using Wolverine;
using Wolverine.RabbitMQ;
using ICommand = Shared.Application.Integration.Commands.ICommand;
using IEvent = Shared.Application.Integration.Events.IEvent;

var host = Host.CreateDefaultBuilder(args)
    // .UseWolverineWithRabbitMq(
    //     new [] { typeof(TestCommandHandler).Assembly }
    //     /*services =>
    //     {
    //         // services.AddHostedService<Worker>();
    //     }*/) // Must be first call because UseWolverine replaces service provider with Lamar https://wolverine.netlify.app/guide/configuration.html#configuration
    .AddRabbitMqSettings()
    .AddServiceSettings()
    // .AddMassTransitWithRabbitMq(new[] { typeof(TestCommandHandler).Assembly })
    .AddRabbitMqBus(new[] { typeof(TestCommandHandler).Assembly })
    .ConfigureServices(
        services =>
            services
                .AddMessageService()
                .AddMessageBus()
                .AddHostedService<Worker>() // TODO Remove
        // TODO Create agents
    )
    .Build();

host.SubscribeQueryHandlers(new[] { typeof(TestCommandHandler).Assembly }/*Routing.SubscribeQueryHandlers*/);
    
host.Run();