using HqService.Service;
using Newtonsoft.Json;
using Shared.Infrastructure;
using Shared.Infrastructure.Exceptions;
using Shared.Infrastructure.Integration.Queries;
using Shared.Infrastructure.Settings;
using Wolverine;
using Wolverine.RabbitMQ;
using ICommand = Shared.Infrastructure.Integration.Commands.ICommand;
using IEvent = Shared.Infrastructure.Integration.Events.IEvent;

var host = Host.CreateDefaultBuilder(args)
    .UseWolverineWithRabbitMq(
        /*services =>
        {
            // services.AddHostedService<Worker>();
        }*/) // Must be first call because UseWolverine replaces service provider with Lamar https://wolverine.netlify.app/guide/configuration.html#configuration
    .AddRabbitMqSettings()
    .ConfigureServices(
        services =>
        {
            services.AddMessageService();
            
            services.AddHostedService<Worker>(); // TODO Remove
            // TODO Create agents
        })
    .Build();

host.Run();