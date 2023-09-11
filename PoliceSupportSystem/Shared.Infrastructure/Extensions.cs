using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using MessageBus.Core;
using MessageBus.Core.API;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;
using Shared.Application;
using Shared.Application.Agents;
using Shared.Application.Handlers;
using Shared.Application.Integration.Commands;
using Shared.Infrastructure.Exceptions;
using Shared.Application.Integration.Queries;
using Shared.Application.Services;
using Shared.Infrastructure.Services;
using Shared.Infrastructure.Settings;
using ExchangeType = RabbitMQ.Client.ExchangeType;
using IBus = MessageBus.Core.API.IBus;
using ICommand = Shared.Application.Integration.Commands.ICommand;
using IEvent = Shared.Application.Integration.Events.IEvent;
using IMessage = Shared.Application.Agents.Communication.Messages.IMessage;
using IMessageBus = Shared.Application.Services.IMessageBus;

namespace Shared.Infrastructure;

// TODO Split this file into many - specialized
public static class Extensions
{
    private const string RabbitMqConfigSectionName = "RabbitMq";
    private const string LokiConfigSectionName = "Loki";

    public static IHostBuilder AddSharedAppSettings(this IHostBuilder hostBuilder) =>
        hostBuilder.ConfigureAppConfiguration(
            (context, config) =>
            {
                var env = context.HostingEnvironment;
                
                string sharedSettingsDirectoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(Extensions))!.Location)!);
                
                config
                    .AddJsonFile(Path.Combine(sharedSettingsDirectoryPath, "sharedsettings.json"), optional: true)
                    .AddJsonFile(Path.Combine(sharedSettingsDirectoryPath, $"sharedsettings.{env.EnvironmentName}.json"), optional: true)
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

                config.AddEnvironmentVariables();
                
            });

    public static IHostBuilder UseAutofac(this IHostBuilder hostBuilder) => hostBuilder.UseServiceProviderFactory(new AutofacServiceProviderFactory());
    
    public static IHostBuilder AddSerilog(this IHostBuilder hostBuilder) => hostBuilder
        .ConfigureLogging((_, loggingBuilder) => loggingBuilder.ClearProviders())
        .UseSerilog(
        (ctx, config) =>
        {
            var serviceSettings = ctx.Configuration.GetSettings<ServiceSettings>(nameof(ServiceSettings));
            var lokiSettings = ctx.Configuration.GetSettings<LokiSettings>(LokiConfigSectionName);

            var lokiCredentials = new LokiCredentials { Login = lokiSettings.Login, Password = lokiSettings.Password };
            var labels = new[] { new LokiLabel { Key = nameof(serviceSettings.Id), Value = serviceSettings.Id } };
            var propertiesAsLabels = Enumerable.Empty<string>();
            
            // var lokiCredentials = new BasicAuthCredentials(lokiSettings.Uri, lokiSettings.Login, lokiSettings.Password);
            // var labels = new LogLabelProvider(new List<LokiLabel> { new LokiLabel(nameof(serviceSettings.Id), serviceSettings.Id) });
            
            // TODO Add logging middleware - log exceptions and so on

            config.MinimumLevel.Verbose();
            
            config.WriteTo.Console();
            config.WriteTo.GrafanaLoki(
                lokiSettings.Uri,
                credentials: lokiCredentials,
                labels: labels,
                propertiesAsLabels: propertiesAsLabels,
                restrictedToMinimumLevel: LogEventLevel.Verbose);
            // config.WriteTo.LokiHttp(lokiCredentials, labels);
        });
    
    public static IServiceCollection AddMessageService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<MessageService>();
        serviceCollection.AddSingleton<IMessageService>(x => x.GetRequiredService<MessageService>());
        serviceCollection.AddSingleton<IMessageHandler>(x => x.GetRequiredService<MessageService>());
        return serviceCollection;
    }

    public static IHostBuilder RegisterSharedApplicationModule(this IHostBuilder hostBuilder) => hostBuilder.RegisterModule<SharedApplicationModule>();

    public static IHostBuilder RegisterModule<TModule>(this IHostBuilder hostBuilder) where TModule : IModule, new() => hostBuilder.ConfigureContainer<ContainerBuilder>(
        builder => builder.RegisterModule<TModule>());

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
                var serviceSettings = context.Configuration.GetSettings<ServiceSettings>(nameof(ServiceSettings));
                services.AddSingleton(serviceSettings);
                services.AddSingleton<IServiceInfoService>(serviceSettings);
            });
    
        return builder;
    }

    public static TSettings GetSettings<TSettings>(this IConfiguration configuration, string sectionName)
    {
        var configSection = configuration.GetRequiredSection(sectionName);
        var settings = configSection.Get<TSettings>() ?? throw new Exception(); // TODO Change exception type
        return settings;
    }

    public static IHostBuilder AddRabbitMqBus(this IHostBuilder hostBuilder, IEnumerable<Assembly> handlerAssemblies/*, Assembly? integrationAssembly = null, Assembly? agentCommunicationAssembly = null*/)
    {
        hostBuilder.ConfigureRabbitMq();
        
        hostBuilder.ConfigureServices(
            (ctx, s) =>
            {
                var rabbitMqSettings = ctx.Configuration.GetSettings<RabbitMqSettings>(RabbitMqConfigSectionName);

                s.AddTransient<IErrorSubscriber, SubscriberErrorService>();
                s.AddSingleton<IBus>(
                    serviceProvider => new RabbitMQBus(
                        configurator =>
                        {
                            configurator.SetConnectionProvidedName("RabbitMQ");
                            configurator.UseConnectionString($"amqp://{rabbitMqSettings.Username}:{rabbitMqSettings.Password}@{rabbitMqSettings.Host}:{rabbitMqSettings.Port}/");
                            configurator.UseErrorSubscriber(serviceProvider.GetRequiredService<IErrorSubscriber>());
                        }));

                s.AddSingleton<IBusSubscriberManager, BusSubscriberManager>();
                
                // s.AddTransient<IBus>(
                //     _ => new RabbitMQBus(
                //         configurator =>
                //         {
                //             configurator.UseConnectionString($"amqp://{rabbitMqSettings.Username}:{rabbitMqSettings.Password}@{rabbitMqSettings.Host}:{rabbitMqSettings.Port}/");
                //         }));

                // TODO Can be improved - multiple handlers with Autofac
                var handlerAssembliesList = handlerAssemblies.ToList();
                if (rabbitMqSettings.QueryExchange is not null)
                {
                    // Register Query Handlers
                    var queryHandlers = DiscoverQueryHandlers(handlerAssembliesList).ToList();
                    queryHandlers.ForEach(x => s.AddScoped(x));
                }

                // TODO Can be improved - multiple handlers with Autofac
                if (rabbitMqSettings.CommandExchange is not null)
                {
                    // Register Command Handlers
                    var commandHandlers = DiscoverCommandHandlers(handlerAssembliesList).ToList();
                    commandHandlers.ForEach(x => s.AddScoped(x));
                }
                
                // TODO Can be improved - multiple handlers with Autofac
                if (rabbitMqSettings.EventExchange is not null)
                {
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

    public static IHost SubscribeHandlers(this IHost host, IList<Assembly> handlerAssemblies)
    {
        var rabbitMqSettings = host.Services.GetRequiredService<RabbitMqSettings>();

        // TODO Can be improved - multiple handlers with Autofac
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
        var subscriberManager = host.Services.GetRequiredService<IBusSubscriberManager>();
        var rabbitMqSettings = host.Services.GetRequiredService<RabbitMqSettings>();
        var serviceSettings = host.Services.GetRequiredService<ServiceSettings>();

        var querySubscriber = subscriberManager.CreateSubscriber(
            x =>
            
                x.SetExchange(rabbitMqSettings.QueryExchange ?? throw new MissingConfigurationException(nameof(rabbitMqSettings.QueryExchange)))
                .SetRoutingKey(serviceSettings.Id)
                .SetConsumerTag(serviceSettings.Id)
                .SetReceiveSelfPublish(false)// TODO Add to config? Resign from it?
            );
        
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
        var subscriberManager = host.Services.GetRequiredService<IBusSubscriberManager>();

        var rabbitMqSettings = host.Services.GetRequiredService<RabbitMqSettings>();
        var serviceSettings = host.Services.GetRequiredService<ServiceSettings>();
        
        var eventSubscriber = subscriberManager.CreateSubscriber(
            x =>
            
                x.SetExchange(rabbitMqSettings.EventExchange ?? throw new MissingConfigurationException(nameof(rabbitMqSettings.EventExchange)))
                    .SetRoutingKey(serviceSettings.Id)
                    .SetConsumerTag(serviceSettings.Id)
                    .SetReceiveSelfPublish(false)// TODO Add to config? Resign from it?
        );

        foreach (var handlerType in DiscoverEventHandlers(handlerAssemblies))
        {
            var eventHandlerInterface = handlerType.GetInterfaces().First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>));
            var eventType = eventHandlerInterface.GetGenericArguments()[0];
            typeof(Extensions).GetMethod(nameof(AddEventHandler), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(handlerType, eventType).Invoke(null, new object[] { eventSubscriber, host.Services });
        }
        
        eventSubscriber.Open();

        return host;
    }
    
    public static IHost SubscribeCommandHandlers(this IHost host, IEnumerable<Assembly> handlerAssemblies /*Action<IAsyncSubscriber, IServiceProvider> action*/)
    {
        var subscriberManager = host.Services.GetRequiredService<IBusSubscriberManager>();
        var rabbitMqSettings = host.Services.GetRequiredService<RabbitMqSettings>();
        var serviceSettings = host.Services.GetRequiredService<ServiceSettings>();

        var commandSubscriber = subscriberManager.CreateSubscriber(
            x =>
            
                x.SetExchange(rabbitMqSettings.CommandExchange ?? throw new MissingConfigurationException(nameof(rabbitMqSettings.CommandExchange)))
                    .SetRoutingKey(serviceSettings.Id)
                    .SetConsumerTag(serviceSettings.Id)
                    .SetReceiveSelfPublish(false)// TODO Add to config? Resign from it?
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
        var subscriberManager = host.Services.GetRequiredService<IBusSubscriberManager>();
        var rabbitMqSettings = host.Services.GetRequiredService<RabbitMqSettings>();
        var serviceSettings = host.Services.GetRequiredService<ServiceSettings>();

        var messageSubscriber = subscriberManager.CreateSubscriber(
            x =>
                x.SetExchange(rabbitMqSettings.MessageExchange ?? throw new MissingConfigurationException(nameof(rabbitMqSettings.MessageExchange)))
                    .SetRoutingKey(serviceSettings.Id)
                    .SetConsumerTag(serviceSettings.Id)
                    .SetReceiveSelfPublish() // TODO Add to config
        );

        foreach (var messageType in DiscoverMessageTypes(new[] { typeof(IMessage).Assembly }))
            typeof(Extensions).GetMethod(nameof(AddMessageHandler), BindingFlags.Static | BindingFlags.NonPublic)!.MakeGenericMethod(messageType)
                .Invoke(null, new object[] { messageSubscriber, host.Services });

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
    
    internal static IAsyncSubscriber AddMessageHandler<TMessage>(this IAsyncSubscriber subscriber, IServiceProvider serviceProvider)
        where TMessage : class, IMessage
    {
        subscriber.Subscribe<TMessage>(x => serviceProvider.GetRequiredService<IMessageHandler>().Handle(x));
        return subscriber;
    }
}