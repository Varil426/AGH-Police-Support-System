using System.Collections;
using System.Reflection;
using Lamar;
using MassTransit;
using MassTransit.Configuration;
using MassTransit.RabbitMqTransport.Topology;
using MessageBus.Core;
using MessageBus.Core.API;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Shared.Application;
using Shared.Application.Agents;
using Shared.Application.Agents.Communication.Messages;
using Shared.Application.Handlers;
using Shared.Application.Integration;
using Shared.Application.Integration.Commands;
using Shared.Application.Integration.Events;
using Shared.Infrastructure.Exceptions;
using Shared.Application.Integration.Queries;
using Shared.Infrastructure.Consumers;
using Shared.Infrastructure.Filters;
using Shared.Infrastructure.Services;
using Shared.Infrastructure.Settings;
using Wolverine;
using Wolverine.Configuration;
using Wolverine.RabbitMQ;
using ExchangeType = RabbitMQ.Client.ExchangeType;
using IBus = MessageBus.Core.API.IBus;
using ICommand = Shared.Application.Integration.Commands.ICommand;
using IEvent = Shared.Application.Integration.Events.IEvent;
using IMessage = Shared.Application.Agents.Communication.Messages.IMessage;
using IMessageBus = Shared.Application.IMessageBus;

namespace Shared.Infrastructure;

// TODO Split this file into many - specialized
public static class Extensions
{
    private const string RabbitMqConfigSectionName = "RabbitMq";

    public static IServiceCollection AddMessageService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<MessageService>();
        serviceCollection.AddSingleton<IMessageService>(x => x.GetRequiredService<MessageService>());
        serviceCollection.AddSingleton<IMessageHandler>(x => x.GetRequiredService<MessageService>());
        return serviceCollection;
    }

    public static IServiceCollection AddMessageBus(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IMessageBus, Services.MessageBus>();
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

    public static IHostBuilder AddServiceSettings(this IHostBuilder builder)
    {
        builder.ConfigureServices(
            (context, services) =>
            {
                var rabbitMqSettings = context.Configuration.GetSettings<ServiceSettings>(nameof(ServiceSettings));
                services.AddSingleton(rabbitMqSettings);
            });
    
        return builder;
    }
    


    // public static IHostBuilder UseWolverineWithRabbitMq(
    //     this IHostBuilder hostBuilder,
    //     IEnumerable<Assembly>? handlerAssemblies = null,
    //     Assembly? integrationAssembly = null,
    //     Assembly? agentCommunicationAssembly = null,
    //     Action<ServiceRegistry>? configureServices = null)
    // {
    //     hostBuilder.UseWolverine(
    //         (context, options) =>
    //         {
    //             // Configure Wolverine
    //             
    //             // configureServices?.Invoke(options.Services); // TODO Here?
    //
    //             // handlerAssemblies ??= Enumerable.Empty<Assembly>();
    //             // foreach (var assembly in handlerAssemblies) // TODO ?
    //             // {
    //             //     options.Discovery.IncludeAssembly(assembly);
    //             // }
    //             // options.Discovery
    //             //     // .DisableConventionalDiscovery() // TODO Should work with this line enabled
    //             //     .CustomizeHandlerDiscovery(
    //             //         x =>
    //             //         {
    //             //             x.Includes.Implements(typeof(ICommandHandler<>));
    //             //             x.Includes.Implements(typeof(ICommandHandler<,>));
    //             //             x.Includes.Implements(typeof(IQueryHandler<,>));
    //             //             x.Includes.Implements(typeof(IEventHandler<>));
    //             //         });
    //                 // .CustomizeMessageDiscovery(
    //                 //     x =>
    //                 //     {
    //                 //         x.Includes.Implements<ICommand>();
    //                 //         x.Includes.Implements(typeof(ICommand<>));
    //                 //         x.Includes.Implements(typeof(IQuery<>));
    //                 //         x.Includes.Implements<IEvent>();
    //                 //         x.Includes.Implements<IMessage>();
    //                 //     });
    //             // Message Discovery
    //             // options.Discovery.IncludeAssembly(typeof(IEvent).Assembly);
    //             // options.Discovery.IncludeAssembly(typeof(IMessage).Assembly);
    //
    //             // Discover Handlers
    //             var queryHandlers = new List<Type>();
    //             var commandHandlers = new List<Type>();
    //             var commandHandlersWithValue = new List<Type>();
    //             var eventHandlers = new List<Type>();
    //
    //             if (handlerAssemblies != null)
    //             {
    //                 foreach (var assembly in handlerAssemblies)
    //                 {
    //                     // options.Discovery.IncludeAssembly(assembly); // TODO ?
    //
    //                     queryHandlers.AddRange(assembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))));
    //                     commandHandlers.AddRange(assembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICommandHandler<>))));
    //                     commandHandlersWithValue.AddRange(assembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))));
    //                     eventHandlers.AddRange(assembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IEventHandler<>))));
    //                 }
    //             }
    //             
    //             // Register handlers
    //             options.Discovery.DisableConventionalDiscovery();
    //             foreach (var handler in queryHandlers.Concat(commandHandlers).Concat(commandHandlersWithValue).Concat(eventHandlers))
    //             {
    //                 options.Discovery.IncludeType(handler);
    //             }
    //
    //             options.Discovery.IncludeType(typeof(MessageHandler));
    //
    //
    //             // Discover Messages, Events, Commands and Queries
    //             var events = new List<Type>();
    //             var commands = new List<Type>();
    //             var queries = new List<Type>();
    //             var messages = new List<Type>();
    //             
    //             integrationAssembly ??= typeof(IEvent).Assembly;
    //             agentCommunicationAssembly ??= typeof(IMessage).Assembly;
    //             events.AddRange(integrationAssembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface == typeof(IEvent))));
    //             commands.AddRange(integrationAssembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface == typeof(ICommand))));
    //             commands.AddRange(integrationAssembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICommand<>))));
    //             queries.AddRange(integrationAssembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IQuery<>))));
    //             messages.AddRange(agentCommunicationAssembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface == typeof(IMessage))));
    //
    //             // options.Discovery.DisableConventionalDiscovery(); // Use attribute WolverineHandler
    //
    //             options.UseNewtonsoftForSerialization(
    //                 newtonsoftSettings =>
    //                 {
    //                     // newtonsoftSettings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
    //                     // newtonsoftSettings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full;
    //                     // newtonsoftSettings.TypeNameHandling = TypeNameHandling.All;
    //                 }); 
    //
    //             // Configure RabbitMQ
    //             var rabbitMqConfig = context.Configuration.GetSettings<RabbitMqSettings>(RabbitMqConfigSectionName);
    //             var serviceConfig = context.Configuration.GetSettings<ServiceSettings>(nameof(ServiceSettings));
    //             options.ServiceName = serviceConfig.Id;
    //             
    //             // var rabbitMqConfig = context.Configuration.GetRequiredSection(_rabbitMqConfigSectionName);
    //             // var exchange = rabbitMqConfig["Exchange"] ?? throw new MissingConfigurationException($"{_rabbitMqConfigSectionName}.Exchange");
    //
    //             // var messageQueueQueue = rabbitMqConfig["MessageQueue"];
    //             // if (rabbitMqConfig.MessageQueue is not null) // TODO Change to queue for each message type, 4 exchanges
    //             // {
    //             //     options.ListenToRabbitQueue(rabbitMqConfig.MessageQueue);//.DefaultIncomingMessage<IMessage>(); // TODO Use this!
    //             //     // options.PublishMessage<IMessage>().ToRabbitQueue(messageQueue);
    //             //     // options.PublishMessage<TestMessage>().ToRabbitQueue(messageQueue);
    //             //     // options.Publish().MessagesFromNamespace(typeof(IMessage).Namespace!).ToRabbitQueue(messageQueue);
    //             //     options.Publish().MessagesFromNamespaceContaining<IMessage>().ToRabbitQueue(rabbitMqConfig.MessageQueue);
    //             //     // options.Publish().MessagesFromNamespaceContaining<TestMessage>().ToRabbitQueue(rabbitMqConfig.MessageQueue);
    //             //     // options.PublishAllMessages();
    //             // }
    //             //
    //             // // var queryQueue = rabbitMqConfig["QueryQueue"];
    //             // if (rabbitMqConfig.QueryQueue is not null)
    //             // {
    //             //     options.ListenToRabbitQueue(rabbitMqConfig.QueryQueue);
    //             //     // options.PublishMessage().ToRabbitQueue(queryQueue); // TODO
    //             //     options.Publish().MessagesFromNamespace(typeof(IQuery<>).Namespace!).ToRabbitQueue(rabbitMqConfig.QueryQueue);
    //             // }
    //             //
    //             // // var commandQueue = rabbitMqConfig["CommandQueue"];
    //             // if (rabbitMqConfig.CommandQueue is not null)
    //             // {
    //             //     options.ListenToRabbitQueue(rabbitMqConfig.CommandQueue);
    //             //     // options.PublishMessage<ICommand>().ToRabbitQueue(commandQueue);
    //             //     options.Publish().MessagesFromNamespaceContaining<ICommand>().ToRabbitQueue(rabbitMqConfig.CommandQueue);
    //             //
    //             // }
    //             //
    //             // // var eventQueue = rabbitMqConfig["EventQueue"];
    //             // if (rabbitMqConfig.EventQueue is not null)
    //             // {
    //             //     options.ListenToRabbitQueue(rabbitMqConfig.EventQueue);
    //             //     // options.PublishMessage<IEvent>().ToRabbitQueue(eventQueue);
    //             //     options.Publish().MessagesFromNamespaceContaining<IEvent>().ToRabbitQueue(rabbitMqConfig.EventQueue);
    //             // }
    //
    //             var transportExpression = options.UseRabbitMq(
    //                     rabbitMq =>
    //                     {
    //                         // rabbitMq.HostName = rabbitMqConfig["Host"] ?? throw new MissingConfigurationException($"{_rabbitMqConfigSectionName}.Host");
    //                         // rabbitMq.Port = Convert.ToInt32(rabbitMqConfig["Port"]);
    //                         
    //                         rabbitMq.HostName = rabbitMqConfig.Host ?? throw new MissingConfigurationException($"{RabbitMqConfigSectionName}.Host");
    //                         rabbitMq.Port = Convert.ToInt32(rabbitMqConfig.Port);
    //                         rabbitMq.UserName = rabbitMqConfig.Username;
    //                         rabbitMq.Password = rabbitMqConfig.Password;
    //                     })
    //                 // // TODO Experiment
    //                 // .UseConventionalRouting(
    //                 //     routingConfig =>
    //                 //     {
    //                 //         routingConfig.ExchangeNameForSending(
    //                 //             x => x switch
    //                 //             {
    //                 //                 not null when x.IsAssignableTo(typeof(IMessage)) => rabbitMqConfig.MessageExchange ??
    //                 //                                                                     throw new MissingConfigurationException(nameof(rabbitMqConfig.MessageExchange)),
    //                 //                 not null when x.IsAssignableTo(typeof(IEvent)) => rabbitMqConfig.EventExchange ??
    //                 //                                                                   throw new MissingConfigurationException(nameof(rabbitMqConfig.EventExchange)),
    //                 //                 not null when x.GetInterfaces().Any(
    //                 //                         @interface => @interface.IsGenericType &&
    //                 //                                       (@interface.GetGenericTypeDefinition() == typeof(ICommand<>) || @interface == typeof(ICommand)))
    //                 //                     => rabbitMqConfig.CommandExchange ?? throw new MissingConfigurationException(nameof(rabbitMqConfig.CommandExchange)),
    //                 //                 not null when x.GetInterfaces().Any(
    //                 //                         @interface => @interface.IsGenericType &&
    //                 //                                       @interface.GetGenericTypeDefinition() == typeof(IQuery<>))
    //                 //                     => rabbitMqConfig.QueryExchange ?? throw new MissingConfigurationException(nameof(rabbitMqConfig.QueryExchange)),
    //                 //                 _ => throw new Exception("Incorrect type")
    //                 //             })
    //                 //             .ConfigureSending(
    //                 //                 (exchangeConfig, messageContext) =>
    //                 //                 {
    //                 //                     switch (messageContext.MessageType)
    //                 //                     {
    //                 //                         case { } x when x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICommand<>) || @interface == typeof(ICommand)):
    //                 //                             // TODO Route to Receiver
    //                 //                             break;
    //                 //                         case {} x when x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IQuery<>)):
    //                 //                             // TODO Route to receiver
    //                 //                             break;
    //                 //                     }
    //                 //                 }).QueueNameForListener();
    //                 //         
    //                 //     })
    //                 // // TODO Experiment
    //                 .AutoProvision()
    //                 .AutoPurgeOnStartup();
    //
    //             // return;
    //             
    //             if (rabbitMqConfig.MessageExchange != null)
    //             {
    //                 transportExpression.DeclareExchange(rabbitMqConfig.MessageExchange, x => x.ExchangeType = ExchangeType.Fanout);
    //                 foreach (var messageType in messages)
    //                 {
    //                     var queueName = /*$"{serviceConfig.Id}_{messageType.Name}";*/$"{rabbitMqConfig.MessageExchange}.{serviceConfig.Id}.{messageType.Name}";
    //                     options.ListenToRabbitQueue(queueName)
    //                         .DefaultIncomingMessage(messageType);
    //                     ((PublishingExpression)typeof(WolverineOptions).GetMethod(nameof(WolverineOptions.PublishMessage))!.MakeGenericMethod(messageType).Invoke(options, null)!)
    //                         .ToRabbitExchange(rabbitMqConfig.MessageExchange);
    //                     transportExpression.BindExchange(rabbitMqConfig.MessageExchange).ToQueue(queueName);
    //                     
    //                     // TODO Somehow handle those messages - pass them to the message service -> agent
    //                 }
    //             }
    //             
    //             if (rabbitMqConfig.EventExchange != null)
    //             {
    //                 transportExpression.DeclareExchange(rabbitMqConfig.EventExchange, x => x.ExchangeType = ExchangeType.Fanout);
    //                 foreach (var eventType in events.Distinct())
    //                 {
    //                     ((PublishingExpression)typeof(WolverineOptions).GetMethod(nameof(WolverineOptions.PublishMessage))!.MakeGenericMethod(eventType).Invoke(options, null)!)
    //                         .ToRabbitExchange(rabbitMqConfig.EventExchange);
    //                     transportExpression.BindExchange(rabbitMqConfig.EventExchange).ToQueue(eventType.Name);
    //                 }
    //
    //                 var eventsToListenTo = eventHandlers
    //                     .SelectMany(x => x.GetInterfaces().Where(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IEventHandler<>)))
    //                     .Select(x => x.GetGenericArguments()[0]).Distinct();
    //                 foreach (var @event in eventsToListenTo)
    //                 {
    //                     var queueName = /*$"{serviceConfig.Id}_{@event.Name}";*/$"{rabbitMqConfig.EventExchange}.{serviceConfig.Id}.{@event.Name}";
    //                     options.ListenToRabbitQueue(queueName).DefaultIncomingMessage(@event);
    //                     transportExpression.BindExchange(rabbitMqConfig.EventExchange).ToQueue(queueName);
    //                 }
    //             }
    //             
    //             if (rabbitMqConfig.QueryExchange != null)
    //             {
    //                 var queueName = /*serviceConfig.Id;*/$"{rabbitMqConfig.QueryExchange}.{serviceConfig.Id}";
    //                 transportExpression.DeclareExchange(rabbitMqConfig.QueryExchange, x => x.ExchangeType = ExchangeType.Topic)
    //                     .BindExchange(rabbitMqConfig.QueryExchange)
    //                     .ToQueue(queueName);
    //                 options.ListenToRabbitQueue(queueName);
    //                 // WithDeliveryOptions(new DeliveryOptions { WithResponse })
    //
    //                 // foreach (var queryType in queries.Distinct())
    //                 // {
    //                 //     ((PublishingExpression)typeof(WolverineOptions).GetMethod(nameof(WolverineOptions.PublishMessage))!.MakeGenericMethod(queryType).Invoke(options, null)!)
    //                 //         .ToRabbitQueue(queryType.Name, x => x.);
    //                 //     transportExpression.BindExchange(rabbitMqConfig.EventExchange).ToQueue(eventType.Name);
    //                 // }
    //
    //                 foreach (var queryType in queries)
    //                 {
    //                     options.Publish(
    //                         x =>
    //                         {
    //                             x.Message(queryType)
    //                                 .ToRabbitExchange(rabbitMqConfig.QueryExchange)
    //                                 // .AddOutgoingRule()
    //                                 // .ToRabbitTopic(string.Empty, rabbitMqConfig.QueryExchange)
    //                                 // .CustomizeOutgoingMessagesOfType<IDirectMessage>(t => t.TopicName = $"{rabbitMqConfig.QueryExchange}_{rabbitMqConfig.QueryExchange}.{serviceConfig.Id}");
    //                             .CustomizeOutgoing(
    //                                 envelope =>
    //                                 {
    //                                     if (envelope.Message is IDirectMessage directMessage)
    //                                     {
    //                                         // envelope.Destination = $"{rabbitMqConfig.QueryExchange}.{directMessage.Receiver}";
    //                                         // envelope.Destination = new Uri($"rabbitmq://queue/{rabbitMqConfig.QueryExchange}_{directMessage.Receiver}");
    //                                         // envelope.Destination = new Uri($"rabbitmq://queue/{directMessage.Receiver}");
    //                                         // envelope.Headers["routing_key"] = $"{rabbitMqConfig.QueryExchange}.{directMessage.Receiver}";
    //                                         // envelope.TopicName = $"{rabbitMqConfig.QueryExchange}.{serviceConfig.Id}";
    //                                         // envelope.Destination = new Uri($"rabbitmq://queue/{rabbitMqConfig.QueryExchange}.{directMessage.Receiver}");
    //                                         envelope.TopicName = $"{rabbitMqConfig.QueryExchange}.{serviceConfig.Id}";
    //                                         envelope.TenantId = "xd xd";
    //                                     }
    //                                         
    //                                 });
    //                         });
    //                 }
    //
    //             }
    //             
    //             if (rabbitMqConfig.CommandExchange != null)
    //             {
    //                 transportExpression.DeclareExchange(rabbitMqConfig.CommandExchange, x => x.ExchangeType = ExchangeType.Direct);
    //             }
    //             
    //             // if (rabbitMqConfig.MessageQueue is not null)
    //             //     transportExpression.BindExchange(rabbitMqConfig.Exchange).ToQueue(rabbitMqConfig.MessageQueue);
    //             //
    //             // if (rabbitMqConfig.QueryQueue is not null)
    //             //     transportExpression.BindExchange(rabbitMqConfig.Exchange).ToQueue(rabbitMqConfig.QueryQueue);
    //             //
    //             // if (rabbitMqConfig.CommandQueue is not null)
    //             //     transportExpression.BindExchange(rabbitMqConfig.Exchange).ToQueue(rabbitMqConfig.CommandQueue);
    //             //
    //             // if (rabbitMqConfig.EventQueue is not null)
    //             //     transportExpression.BindExchange(rabbitMqConfig.Exchange).ToQueue(rabbitMqConfig.EventQueue);
    //
    //             configureServices?.Invoke(options.Services); // TODO Here?
    //             
    //             // Console.WriteLine(options.DescribeHandlerMatch(typeof(TestMessage)));
    //             // Console.WriteLine(options.DescribeHandlerMatch(typeof(TestEvent)));
    //             // Console.WriteLine(options.DescribeHandlerMatch(eventHandlers[0]));
    //             Console.WriteLine(options.DescribeHandlerMatch(typeof(MessageHandler)));
    //         });
    //
    //     return hostBuilder;
    // }

    public static IHostBuilder AddMassTransitWithRabbitMq(this IHostBuilder builder, IEnumerable<Assembly> handlerAssemblies, Assembly? integrationAssembly = null, Assembly? agentCommunicationAssembly = null)
    {
        builder.ConfigureServices(
            services =>
            {
                services.AddMassTransit(
                    massTransitConfiguration =>
                    {
                        ConsumerConvention.Register<Shared.Infrastructure.Conventions.ConsumerConvention>();
                        ///
                        // handlerAssemblies.ToList().ForEach(x => massTransitConfiguration.AddConsumers(x));
                        var queryHandlers = new List<Type>();
                        var commandHandlers = new List<Type>();
                        // var commandHandlersWithValue = new List<Type>();
                        var eventHandlers = new List<Type>();
                        
                        foreach (var assembly in handlerAssemblies)
                        {
                            queryHandlers.AddRange(assembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))));
                            commandHandlers.AddRange(assembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICommandHandler<>))));
                            // commandHandlersWithValue.AddRange(assembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))));
                            commandHandlers.AddRange(assembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))));
                            eventHandlers.AddRange(assembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IEventHandler<>))));
                        }

                        queryHandlers.ForEach(
                            x =>
                            {
                                services.AddTransient(x);
                                // var queryHandlerInterface = x.GetInterfaces().First(
                                //     @interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));
                                // var queryType = queryHandlerInterface.GetGenericArguments()[0];
                                // var resultType = queryHandlerInterface.GetGenericArguments()[1];
                                // massTransitConfiguration.AddConsumer(typeof(GenericQueryConsumer<,,>).MakeGenericType(x, queryType, resultType));
                            });

                        
                        var events = new List<Type>();
                        var commands = new List<Type>();
                        var queries = new List<Type>();
                        var messages = new List<Type>();
                        integrationAssembly ??= typeof(IEvent).Assembly;
                        agentCommunicationAssembly ??= typeof(IMessage).Assembly;
                        events.AddRange(integrationAssembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface == typeof(IEvent))));
                        commands.AddRange(integrationAssembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface == typeof(ICommand))));
                        commands.AddRange(integrationAssembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICommand<>))));
                        queries.AddRange(integrationAssembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IQuery<>))));
                        messages.AddRange(agentCommunicationAssembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface == typeof(IMessage))));
                        
                        
                        ///
                        massTransitConfiguration.AddRabbitMqBus(queryHandlers, queries /*handlerAssemblies, communicationAssemblies*/);
                        
                        
                    });
            });
        
        
        return builder;
    }
    
    public static TSettings GetSettings<TSettings>(this IConfiguration configuration, string sectionName) /*where TSettings : new()*/ // TODO Remove commented code
    {
        var configSection = configuration.GetRequiredSection(sectionName);
        var settings = configSection.Get<TSettings>() ?? throw new Exception(); // TODO Change exception type
        return settings;
    }

    private static IBusRegistrationConfigurator AddRabbitMqBus(this IBusRegistrationConfigurator configurator, /*IEnumerable<Assembly> handlerAssemblies, IEnumerable<Assembly> communicationAssemblies*/ IEnumerable<Type> handlers, IEnumerable<Type> queries)
    {
        configurator.UsingRabbitMq(
            (context, config) =>
            {
                var rabbitMqConfig = context.GetRequiredService<RabbitMqSettings>();
                var serviceConfig = context.GetRequiredService<ServiceSettings>();

                // Configure Host
                config.Host(rabbitMqConfig.Host, rabbitMqConfig.Port, "/",
                    x =>
                    {
                        x.Username(rabbitMqConfig.Username);
                        x.Username(rabbitMqConfig.Password);
                    });
                
                config.UseSendFilter(typeof(QueryOutgoingFilter<>), context);
                // config.ReceiveEndpoint(serviceConfig.Id,
                //     e =>
                //     {
                //         // e.ExchangeType = RabbitMqExchangeTypeEnum.Direct.ToString().ToLower();
                //
                //         foreach (var handler in handlers)
                //         {
                //             e.Consumer(handler, context.GetRequiredService);
                //         }
                //
                //         foreach (var query in queries)
                //         {
                //             e.Bind($"{query.Namespace}:{query.Name}",
                //                 x =>
                //                 {
                //                     x.Durable = false;
                //                     x.AutoDelete = true;
                //                     x.ExchangeType = ExchangeType.Direct.ToString();//RabbitMqExchangeTypeEnum.Direct.ToString().ToLower();;
                //                     x.RoutingKey = serviceConfig.Id;
                //                 });
                //         }
                //         
                //         e.ConfigureConsumers(context);
                //     });

             
                config.ReceiveEndpoint(serviceConfig.Id,
                    e =>
                    {
                        // e.Bind<TestQuery>(
                        //     x =>
                        //     {
                        //         x.RoutingKey = serviceConfig.Id;
                        //         x.ExchangeType = ExchangeType.Direct.ToString().ToLower();
                        //         x.Durable = false; // TODO
                        //         x.AutoDelete = true; // TODO
                        //     });
                    });
                
                
                
                config.ConfigureEndpoints(context);
                
                config.Send<TestQuery>(
                    x =>
                    {
                        x.UseRoutingKeyFormatter(m => m.Message.Receiver);
                    });
                

                // config.Host(new Uri($"rabbitmq://{rabbitMqConfig.Host}:{rabbitMqConfig.Port}/"),
                //     x =>
                //     {
                //         x.Username(rabbitMqConfig.Username);
                //         x.Username(rabbitMqConfig.Password);
                //     });
                
                // Discover Messages, Events, Commands and Queries
                // var events = new List<Type>();
                // var commands = new List<Type>();
                // var queries = new List<Type>();
                // var messages = new List<Type>();
                // var integrationAssembly = typeof(IEvent).Assembly; // TODO Move to params, ??=
                // var agentCommunicationAssembly = typeof(IMessage).Assembly; // TODO Move to params, ??=
                // events.AddRange(integrationAssembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface == typeof(IEvent))));
                // commands.AddRange(integrationAssembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface == typeof(ICommand))));
                // commands.AddRange(integrationAssembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICommand<>))));
                // queries.AddRange(integrationAssembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IQuery<>))));
                // messages.AddRange(agentCommunicationAssembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface == typeof(IMessage))));
                //
                // // Create exchanges
                // events.ForEach(x => config.Publish(x,
                //     publishConfig =>
                //     {
                //         publishConfig.ExchangeType = RabbitMqExchangeTypeEnum.Fanout.ToString();
                //     }));
                // commands.ForEach(
                //     x =>
                //     {
                //         Action<IRabbitMqMessageSendTopologyConfigurator<ICommand>> c = publishConfig =>
                //         {
                //             publishConfig. = RabbitMqExchangeTypeEnum.Direct.ToString();
                //             publishConfig.UseRoutingKeyFormatter
                //         };
                //         config.GetType().GetMethod(nameof(config.Send))!.MakeGenericMethod(x).Invoke(config, ))
                //
                //     });
                // queries.ForEach(x => config.Publish(x,
                //     publishConfig =>
                //     {
                //         publishConfig.ExchangeType = RabbitMqExchangeTypeEnum.Direct.ToString();
                //     }));
                // messages.ForEach(x => config.Publish(x,
                //     publishConfig =>
                //     {
                //         publishConfig.ExchangeType = RabbitMqExchangeTypeEnum.Topic.ToString();
                //     }));
                //
                // // Discover Handlers
                // var queryHandlers = new List<Type>();
                // var commandHandlers = new List<Type>();
                // // var commandHandlersWithValue = new List<Type>();
                // var eventHandlers = new List<Type>();
                //
                // foreach (var assembly in handlerAssemblies)
                // {
                //     queryHandlers.AddRange(assembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))));
                //     commandHandlers.AddRange(assembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICommandHandler<>))));
                //     // commandHandlersWithValue.AddRange(assembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))));
                //     commandHandlers.AddRange(assembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))));
                //     eventHandlers.AddRange(assembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IEventHandler<>))));
                // }
                //
                // var eventsToListenTo = eventHandlers
                //     .SelectMany(x => x.GetInterfaces().Where(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IEventHandler<>)))
                //     .Select(x => x.GetGenericArguments()[0]).Distinct();
                //
                // var commandsToListenTo = commandHandlers
                //     .SelectMany(x => x.GetInterfaces().Where(@interface => @interface.IsGenericType && (@interface.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                //                                                                                         @interface.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))))
                //     .Select(x => x.GetGenericArguments()[0]).Distinct();
                //
                // var queriesToListenTo = commandHandlers
                //     .SelectMany(x => x.GetInterfaces().Where(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
                //     .Select(x => x.GetGenericArguments()[0]).Distinct();
                //
                // // TODO Discover rest of types
                //
                // config.rece
            });

        return configurator;
    }

    public static IHostBuilder AddRabbitMqBus(this IHostBuilder hostBuilder, IEnumerable<Assembly> handlerAssemblies, Assembly? integrationAssembly = null, Assembly? agentCommunicationAssembly = null)
    {
        hostBuilder.ConfigureRabbitMq();
        
        hostBuilder.ConfigureServices(
            (ctx, s) =>
            {
                var rabbitMqSettings = ctx.Configuration.GetSettings<RabbitMqSettings>(RabbitMqConfigSectionName);
                var serviceSettings = ctx.Configuration.GetSettings<ServiceSettings>(nameof(ServiceSettings));
                
                var bus = new RabbitMQBus(
                    configurator =>
                    {
                        configurator.UseConnectionString($"amqp://{rabbitMqSettings.Username}:{rabbitMqSettings.Password}@{rabbitMqSettings.Host}:{rabbitMqSettings.Port}/");
                    });
                s.AddSingleton<IBus>(bus);

                var handlerAssembliesList = handlerAssemblies.ToList();
                if (rabbitMqSettings.QueryExchange is not null)
                {
                    // var querySubscriber = bus.CreateAsyncSubscriber(
                    //     x =>
                    //     {
                    //         x.SetExchange(rabbitMqSettings.QueryExchange);
                    //         x.SetRoutingKey(serviceSettings.Id);
                    //     });


                    // Register Query Handlers
                    var queryHandlers = DiscoverQueryHandlers(handlerAssembliesList).ToList();
                    queryHandlers.ForEach(x => s.AddScoped(x));
                    
                    
                    // Discover Handled Queries
                    // var handledQueries = DiscoverHandledQueries(queryHandlers);
                    // foreach (var handledQueryType in handledQueries.Keys)
                    // {
                    //     querySubscriber.Subscribe(
                    //         handledQueryType,
                    //         x => (Task)handledQueries[handledQueryType].GetMethod(nameof(IQueryHandler<IQuery<object>, object>.Handle))!
                    //             .Invoke(s.BuildServiceProvider().GetRequiredService(handledQueries[handledQueryType]), new[] { x })!);
                    // }
                    //
                    // querySubscriber.Open();
                }

                if (rabbitMqSettings.CommandExchange is not null)
                {
                    // TODO
                    // Register Command Handlers
                    var commandHandlers = DiscoverCommandHandlers(handlerAssembliesList).ToList();
                    commandHandlers.ForEach(x => s.AddScoped(x));
                }




                if (rabbitMqSettings.EventExchange is not null)
                {
                    // TODO
                    // Register Event Handlers
                    var eventHandlers = DiscoverEventHandlers(handlerAssembliesList).ToList();
                    eventHandlers.ForEach(x => s.AddScoped(x));
                }
                        
                
            });
        
        
        return hostBuilder;
    }

    public static IHostBuilder ConfigureRabbitMq(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices(
            (ctx, s) =>
            {
                var rabbitMqSettings = ctx.Configuration.GetSettings<RabbitMqSettings>(RabbitMqConfigSectionName);
                var factory = new ConnectionFactory
                    { HostName = rabbitMqSettings.Host, Password = rabbitMqSettings.Password, UserName = rabbitMqSettings.Username, Port = rabbitMqSettings.Port };
                using var connection = factory.CreateConnection();
                using var model = connection.CreateModel();

                if (rabbitMqSettings.QueryExchange is not null)
                    model.ExchangeDeclare(rabbitMqSettings.QueryExchange, ExchangeType.Direct, false, true);
                if (rabbitMqSettings.CommandExchange is not null)
                    model.ExchangeDeclare(rabbitMqSettings.CommandExchange, ExchangeType.Direct, false, true);
                if (rabbitMqSettings.EventExchange is not null)
                    model.ExchangeDeclare(rabbitMqSettings.EventExchange, ExchangeType.Fanout, false, true);
                if (rabbitMqSettings.DirectMessageExchange is not null)
                    model.ExchangeDeclare(rabbitMqSettings.DirectMessageExchange, ExchangeType.Direct, false, true);
                if (rabbitMqSettings.MessageExchange is not null)
                    model.ExchangeDeclare(rabbitMqSettings.MessageExchange, ExchangeType.Fanout, false, true);
            });
        
        return hostBuilder;
    }

    internal static IEnumerable<Type> DiscoverMessageTypes(IEnumerable<Assembly> assemblies)
    {
        var messageTypes = new List<Type>();
        foreach (var assembly in assemblies)
            messageTypes.AddRange(
                assembly.GetTypes().Where(
                    x => !x.IsAbstract && x.IsAssignableTo(typeof(IMessage))));
        return messageTypes;
    }
    
    private static IEnumerable<Type> DiscoverQueryHandlers(IEnumerable<Assembly> assemblies)
    {
        var queryHandlers = new List<Type>();
        foreach (var assembly in assemblies)
            queryHandlers.AddRange(
                assembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))));
        return queryHandlers;
    }
    
    private static IEnumerable<Type> DiscoverCommandHandlers(IEnumerable<Assembly> assemblies)
    {
        var commandHandlers = new List<Type>();
        foreach (var assembly in assemblies)
            commandHandlers.AddRange(
                assembly.GetTypes().Where(
                    x => x.GetInterfaces().Any(
                        @interface => @interface.IsGenericType &&
                                      (@interface.GetGenericTypeDefinition() == typeof(ICommandHandler<>) || @interface.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)))));
        return commandHandlers;
    }

    private static IEnumerable<Type> DiscoverEventHandlers(IEnumerable<Assembly> assemblies)
    {
        var eventHandlers = new List<Type>();
        foreach (var assembly in assemblies)
            eventHandlers.AddRange(
                assembly.GetTypes().Where(x => x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IEventHandler<>))));
        return eventHandlers;
    }

    private static IDictionary<Type, Type> DiscoverHandledQueries(IEnumerable<Type> queryHandlers)
    {
        // Dictionary Query -> Handler
        var result = new Dictionary<Type, Type>();

        foreach (var queryHandler in queryHandlers)
        {
            foreach (var @interface in queryHandler.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
            {
                var queryType = @interface.GetGenericArguments()[0];
                result.Add(queryType, queryHandler);
            }
        }

        return result;
    }

    public static IHost SubscribeHandlers(this IHost host, IList<Assembly> handlerAssemblies)
    {
        var rabbitMqSettings = host.Services.GetRequiredService<RabbitMqSettings>();

        if (rabbitMqSettings.QueryExchange is not null)
            host.SubscribeQueryHandlers(handlerAssemblies);
        
        if (rabbitMqSettings.EventExchange is not null)
            host.SubscribeEventHandlers(handlerAssemblies);
        
        if (rabbitMqSettings.CommandExchange is not null)
            host.SubscribeCommandHandlers(handlerAssemblies);
        
        if (rabbitMqSettings.MessageExchange is not null)
            host.SubscribeMessageService();

        return host;
    }
    
    public static IHost SubscribeQueryHandlers(this IHost host, IEnumerable<Assembly> handlerAssemblies /*Action<IAsyncSubscriber, IServiceProvider> action*/)
    {
        var bus = host.Services.GetRequiredService<IBus>();
        var rabbitMqSettings = host.Services.GetRequiredService<RabbitMqSettings>();
        var serviceSettings = host.Services.GetRequiredService<ServiceSettings>();

        var querySubscriber = bus.CreateAsyncSubscriber(
            x =>
            
                x.SetExchange(rabbitMqSettings.QueryExchange ?? throw new MissingConfigurationException(nameof(rabbitMqSettings.QueryExchange)))
                .SetRoutingKey(serviceSettings.Id)
                .SetConsumerTag(serviceSettings.Id)
                .SetReceiveSelfPublish() // TODO Add to config
            );

        // action.Invoke(querySubscriber, host.Services);

        foreach (var handlerType in DiscoverQueryHandlers(handlerAssemblies))
        {
            var queryHandlerInterface = handlerType.GetInterfaces().First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));
            var queryType = queryHandlerInterface.GetGenericArguments()[0];
            var resultType = queryHandlerInterface.GetGenericArguments()[1];
            typeof(Extensions).GetMethod(nameof(AddQueryHandler), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(handlerType, queryType, resultType).Invoke(null, new object[] { querySubscriber, host.Services });
        }
        
        querySubscriber.Open();

        return host;
    }
    
    public static IHost SubscribeEventHandlers(this IHost host, IEnumerable<Assembly> handlerAssemblies /*Action<IAsyncSubscriber, IServiceProvider> action*/)
    {
        var bus = host.Services.GetRequiredService<IBus>();
        var rabbitMqSettings = host.Services.GetRequiredService<RabbitMqSettings>();
        var serviceSettings = host.Services.GetRequiredService<ServiceSettings>();

        var eventSubscriber = bus.CreateAsyncSubscriber(
            x =>
            
                x.SetExchange(rabbitMqSettings.EventExchange ?? throw new MissingConfigurationException(nameof(rabbitMqSettings.EventExchange)))
                    .SetRoutingKey(serviceSettings.Id)
                    .SetConsumerTag(serviceSettings.Id)
                    .SetReceiveSelfPublish() // TODO Add to config
        );

        // action.Invoke(querySubscriber, host.Services);

        foreach (var handlerType in DiscoverEventHandlers(handlerAssemblies))
        {
            var queryHandlerInterface = handlerType.GetInterfaces().First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>));
            var eventType = queryHandlerInterface.GetGenericArguments()[0];
            typeof(Extensions).GetMethod(nameof(AddEventHandler), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(handlerType, eventType).Invoke(null, new object[] { eventSubscriber, host.Services });
        }
        
        eventSubscriber.Open();

        return host;
    }
    
    public static IHost SubscribeCommandHandlers(this IHost host, IEnumerable<Assembly> handlerAssemblies /*Action<IAsyncSubscriber, IServiceProvider> action*/)
    {
        var bus = host.Services.GetRequiredService<IBus>();
        var rabbitMqSettings = host.Services.GetRequiredService<RabbitMqSettings>();
        var serviceSettings = host.Services.GetRequiredService<ServiceSettings>();

        var commandSubscriber = bus.CreateAsyncSubscriber(
            x =>
            
                x.SetExchange(rabbitMqSettings.CommandExchange ?? throw new MissingConfigurationException(nameof(rabbitMqSettings.CommandExchange)))
                    .SetRoutingKey(serviceSettings.Id)
                    .SetConsumerTag(serviceSettings.Id)
                    .SetReceiveSelfPublish() // TODO Add to config
        );
        
        foreach (var handlerType in DiscoverCommandHandlers(handlerAssemblies))
        {
            var commandHandlerInterface = handlerType.GetInterfaces().First(x => x.IsGenericType &&
                                                                               (x.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                                                                                x.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)));
            var commandType = commandHandlerInterface.GetGenericArguments()[0];
            if (commandHandlerInterface.GetGenericArguments().Length == 2)
            {
                var resultType = commandHandlerInterface.GetGenericArguments()[1];
                typeof(Extensions).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).First(x => x.Name == nameof(AddCommandHandler) && x.GetGenericArguments().Length == 3)
                    .MakeGenericMethod(handlerType, commandType, resultType).Invoke(null, new object[] { commandSubscriber, host.Services });
            }
            else
                typeof(Extensions).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).First(x => x.Name == nameof(AddCommandHandler) && x.GetGenericArguments().Length == 2)
                    .MakeGenericMethod(handlerType, commandType).Invoke(null, new object[] { commandSubscriber, host.Services });
        }
        
        commandSubscriber.Open();

        return host;
    }

    public static IHost SubscribeMessageService(this IHost host)
    {
        var bus = host.Services.GetRequiredService<IBus>();
        var rabbitMqSettings = host.Services.GetRequiredService<RabbitMqSettings>();
        var serviceSettings = host.Services.GetRequiredService<ServiceSettings>();

        var messageSubscriber = bus.CreateAsyncSubscriber(
            x =>
                x.SetExchange(rabbitMqSettings.MessageExchange ?? throw new MissingConfigurationException(nameof(rabbitMqSettings.MessageExchange)))
                    .SetRoutingKey(serviceSettings.Id)
                    .SetConsumerTag(serviceSettings.Id)
                    .SetReceiveSelfPublish() // TODO Add to config
        );

        foreach (var messageType in DiscoverMessageTypes(new[] { typeof(IMessage).Assembly }))
            typeof(Extensions).GetMethod(nameof(AddMessageHandler), BindingFlags.Static | BindingFlags.NonPublic)!.MakeGenericMethod(messageType)
                .Invoke(null, new object[] { messageSubscriber, host.Services });

        // foreach (var messageType in DiscoverMessageTypes(new[] { typeof(IMessage).Assembly }))
        //     messageSubscriber.AddMessageHandler(host.Services, messageType);

        messageSubscriber.Open();
        
        return host;
    }

    private static IAsyncSubscriber AddQueryHandler<THandler, TQuery, TResult>(this IAsyncSubscriber subscriber, IServiceProvider serviceProvider)
        where THandler : class, IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>
    {
        subscriber.Subscribe<TQuery, TResult>(
            async x =>
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                return await scope.ServiceProvider.GetRequiredService<THandler>().Handle(x);
            });
        return subscriber;
    }
    
    private static IAsyncSubscriber AddEventHandler<THandler, TEvent>(this IAsyncSubscriber subscriber, IServiceProvider serviceProvider)
        where THandler : class, IEventHandler<TEvent>
        where TEvent : class, IEvent
    {
        subscriber.Subscribe<TEvent>(
            async x =>
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                await scope.ServiceProvider.GetRequiredService<THandler>().Handle(x);
            });
        return subscriber;
    }

    private static IAsyncSubscriber AddCommandHandler<THandler, TCommand, TResult>(this IAsyncSubscriber subscriber, IServiceProvider serviceProvider)
        where THandler : class, ICommandHandler<TCommand, TResult>
        where TCommand : class, ICommand<TResult>
    {
        subscriber.Subscribe<TCommand, TResult>(
            async x =>
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                return await scope.ServiceProvider.GetRequiredService<THandler>().Handle(x);
            });
        return subscriber;
    }
    
    private static IAsyncSubscriber AddCommandHandler<THandler, TCommand>(this IAsyncSubscriber subscriber, IServiceProvider serviceProvider)
        where THandler : class, ICommandHandler<TCommand>
        where TCommand : class, ICommand
    {
        subscriber.Subscribe<TCommand>(
            async x =>
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                await scope.ServiceProvider.GetRequiredService<THandler>().Handle(x);
            });
        return subscriber;
    }

    // private static IEnumerable<T> GetAll<T> (this IServiceProvider provider)
    // {
    //     var site = typeof(ServiceProvider).GetProperty("CallSiteFactory", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(provider);
    //     var desc = site.GetType().GetField("_descriptors", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(site) as ServiceDescriptor[];
    //     return desc.Select(s => provider.GetRequiredService(s.ServiceType)).OfType<T>();
    // }
    
    internal static IAsyncSubscriber AddMessageHandler<TMessage>(this IAsyncSubscriber subscriber, IServiceProvider serviceProvider)
        where TMessage : class, IMessage
    {
        subscriber.Subscribe<TMessage>(x => serviceProvider.GetRequiredService<IMessageHandler>().Handle(x));
        return subscriber;
    }
    
    // private static IAsyncSubscriber AddMessageHandler(this IAsyncSubscriber subscriber, IServiceProvider serviceProvider, Type messageType)
    // {
    //     subscriber.Subscribe(messageType, o => serviceProvider.GetRequiredService<IMessageHandler>().Handle((IMessage)o));
    //     return subscriber;
    // }
}