using Lamar;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Shared.Agents;
using Shared.Infrastructure.Exceptions;
using Shared.Infrastructure.Integration.Queries;
using Shared.Infrastructure.Services;
using Shared.Infrastructure.Settings;
using Wolverine;
using Wolverine.RabbitMQ;
using ICommand = Shared.Infrastructure.Integration.Commands.ICommand;
using IEvent = Shared.Infrastructure.Integration.Events.IEvent;
using IMessage = Shared.Agents.Communication.Messages.IMessage;

namespace Shared.Infrastructure;

public static class Extensions
{
    private const string RabbitMqConfigSectionName = "RabbitMq";

    public static IServiceCollection AddMessageService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IMessageService, MessageService>();
        return serviceCollection;
    }

    public static IHostBuilder AddRabbitMqSettings(this IHostBuilder builder)
    {
        builder.ConfigureServices(
            (context, services) =>
            {
                var rabbitMqSettings = context.Configuration.GetSettings<RabbitMqSettings>(RabbitMqConfigSectionName);
                services.AddSingleton(rabbitMqSettings);
            });

        return builder;
    }
    
    public static IHostBuilder UseWolverineWithRabbitMq(this IHostBuilder hostBuilder, Action<ServiceRegistry>? configureServices = null)
    {
        hostBuilder.UseWolverine(
            (context, options) =>
            {
                // Configure Wolverine

                // options.Discovery
                //     .DisableConventionalDiscovery()
                //     .include
                // TODO

                options.Discovery.DisableConventionalDiscovery(); // Use attribute WolverineHandler
                
                options.UseNewtonsoftForSerialization(
                    newtonsoftSettings =>
                    {
                        // newtonsoftSettings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
                        // newtonsoftSettings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full;
                        // newtonsoftSettings.TypeNameHandling = TypeNameHandling.All;
                    });

                // Configure RabbitMQ
                var rabbitMqConfig = context.Configuration.GetSettings<RabbitMqSettings>(RabbitMqConfigSectionName);
                // var rabbitMqConfig = context.Configuration.GetRequiredSection(_rabbitMqConfigSectionName);
                // var exchange = rabbitMqConfig["Exchange"] ?? throw new MissingConfigurationException($"{_rabbitMqConfigSectionName}.Exchange");
                
                
                
                // var messageQueueQueue = rabbitMqConfig["MessageQueue"];
                if (rabbitMqConfig.MessageQueue is not null) // TODO Change to queue for each message type, 4 exchanges
                {
                    options.ListenToRabbitQueue(rabbitMqConfig.MessageQueue);//.DefaultIncomingMessage<IMessage>(); // TODO Use this!
                    // options.PublishMessage<IMessage>().ToRabbitQueue(messageQueue);
                    // options.PublishMessage<TestMessage>().ToRabbitQueue(messageQueue);
                    // options.Publish().MessagesFromNamespace(typeof(IMessage).Namespace!).ToRabbitQueue(messageQueue);
                    options.Publish().MessagesFromNamespaceContaining<IMessage>().ToRabbitQueue(rabbitMqConfig.MessageQueue);
                    // options.Publish().MessagesFromNamespaceContaining<TestMessage>().ToRabbitQueue(rabbitMqConfig.MessageQueue);
                    // options.PublishAllMessages();
                }

                // var queryQueue = rabbitMqConfig["QueryQueue"];
                if (rabbitMqConfig.QueryQueue is not null)
                {
                    options.ListenToRabbitQueue(rabbitMqConfig.QueryQueue);
                    // options.PublishMessage().ToRabbitQueue(queryQueue); // TODO
                    options.Publish().MessagesFromNamespace(typeof(IQuery<>).Namespace!).ToRabbitQueue(rabbitMqConfig.QueryQueue);
                }
                
                // var commandQueue = rabbitMqConfig["CommandQueue"];
                if (rabbitMqConfig.CommandQueue is not null)
                {
                    options.ListenToRabbitQueue(rabbitMqConfig.CommandQueue);
                    // options.PublishMessage<ICommand>().ToRabbitQueue(commandQueue);
                    options.Publish().MessagesFromNamespaceContaining<ICommand>().ToRabbitQueue(rabbitMqConfig.CommandQueue);

                }
                
                // var eventQueue = rabbitMqConfig["EventQueue"];
                if (rabbitMqConfig.EventQueue is not null)
                {
                    options.ListenToRabbitQueue(rabbitMqConfig.EventQueue);
                    // options.PublishMessage<IEvent>().ToRabbitQueue(eventQueue);
                    options.Publish().MessagesFromNamespaceContaining<IEvent>().ToRabbitQueue(rabbitMqConfig.EventQueue);
                }

                var transportExpression = options.UseRabbitMq(
                        rabbitMq =>
                        {
                            // rabbitMq.HostName = rabbitMqConfig["Host"] ?? throw new MissingConfigurationException($"{_rabbitMqConfigSectionName}.Host");
                            // rabbitMq.Port = Convert.ToInt32(rabbitMqConfig["Port"]);
                            
                            rabbitMq.HostName = rabbitMqConfig.Host ?? throw new MissingConfigurationException($"{RabbitMqConfigSectionName}.Host");
                            rabbitMq.Port = Convert.ToInt32(rabbitMqConfig.Port);
                            rabbitMq.UserName = rabbitMqConfig.Username;
                            rabbitMq.Password = rabbitMqConfig.Password;
                        })
                    .AutoProvision()
                    .AutoPurgeOnStartup();

                if (rabbitMqConfig.MessageQueue is not null)
                    transportExpression.BindExchange(rabbitMqConfig.Exchange).ToQueue(rabbitMqConfig.MessageQueue);
                
                if (rabbitMqConfig.QueryQueue is not null)
                    transportExpression.BindExchange(rabbitMqConfig.Exchange).ToQueue(rabbitMqConfig.QueryQueue);
                
                if (rabbitMqConfig.CommandQueue is not null)
                    transportExpression.BindExchange(rabbitMqConfig.Exchange).ToQueue(rabbitMqConfig.CommandQueue);
                
                if (rabbitMqConfig.EventQueue is not null)
                    transportExpression.BindExchange(rabbitMqConfig.Exchange).ToQueue(rabbitMqConfig.EventQueue);

                configureServices?.Invoke(options.Services);
            });

        return hostBuilder;
    }
    
    public static TSettings GetSettings<TSettings>(this IConfiguration configuration, string sectionName) /*where TSettings : new()*/ // TODO Remove commented code
    {
        var configSection = configuration.GetRequiredSection(sectionName);
        var settings = configSection.Get<TSettings>() ?? throw new Exception(); // TODO Change exception type
        return settings;
    }
}