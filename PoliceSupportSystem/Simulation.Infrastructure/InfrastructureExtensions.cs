using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MessageBus.Core;
using MessageBus.Core.API;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using RabbitMQ.Client;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;
using Simulation.Application;
using Simulation.Application.Directors.Settings;
using Simulation.Application.Handlers;
using Simulation.Application.Handlers.Messages;
using Simulation.Application.Handlers.Queries;
using Simulation.Application.Services;
using Simulation.Communication.Messages;
using Simulation.Communication.Queries;
using Simulation.Infrastructure.Exceptions;
using Simulation.Infrastructure.Services;
using Simulation.Infrastructure.Settings;

namespace Simulation.Infrastructure;

public static class InfrastructureExtensions
{
    public static IHostBuilder UseAutofac(this IHostBuilder hostBuilder) => hostBuilder.UseServiceProviderFactory(new AutofacServiceProviderFactory());

    public static IHostBuilder AddSettings(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices(
            (ctx, serviceCollection) =>
            {
                var rabbitMqSettings = ctx.Configuration.GetSettings<RabbitMqSettings>(nameof(RabbitMqSettings));
                var simulationSettings = ctx.Configuration.GetSettings<SimulationSettings>(nameof(SimulationSettings));
                var incidentDirectorSettings = ctx.Configuration.GetSettings<IncidentDirectorSettings>(nameof(IncidentDirectorSettings));
                var patrolDirectorSettings = ctx.Configuration.GetSettings<PatrolDirectorSettings>(nameof(PatrolDirectorSettings));
                var dbSettings = ctx.Configuration.GetSettings<DbSettings>(nameof(DbSettings));

                serviceCollection.AddSingleton(rabbitMqSettings);
                serviceCollection.AddSingleton(simulationSettings);
                serviceCollection.AddSingleton(incidentDirectorSettings);
                serviceCollection.AddSingleton(patrolDirectorSettings);
                serviceCollection.AddSingleton(dbSettings);
            });
        return hostBuilder;
    }

    public static TSettings GetSettings<TSettings>(this IConfiguration configuration, string sectionName)
    {
        var configSection = configuration.GetRequiredSection(sectionName);
        var settings = configSection.Get<TSettings>() ?? throw new Exception(); // TODO Change exception type
        return settings;
    }

    public static IHostBuilder AddNpgsql(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices(
            (ctx, s) =>
            {
                s.AddSingleton<NpgsqlDataSourceBuilder>(
                    x =>
                    {
                        var settings = x.GetRequiredService<DbSettings>();
                        return new NpgsqlDataSourceBuilder(
                            $"Host={settings.Host};Port={settings.Port};Username={settings.Username};Password={settings.Password};Database={settings.DbName}");
                    });
                s.AddSingleton<NpgsqlDataSource>(
                    x =>
                        x.GetRequiredService<NpgsqlDataSourceBuilder>().Build()
                );
            });

        return hostBuilder;
    }
    
    public static IHostBuilder AddRabbitMqBus(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureRabbitMq();
        
        hostBuilder.ConfigureServices(
            (ctx, s) =>
            {
                var rabbitMqSettings = ctx.Configuration.GetSettings<RabbitMqSettings>(nameof(RabbitMqSettings));
                s.AddSingleton<IBus>(
                    _ => new RabbitMQBus(
                        configurator =>
                        {
                            configurator.SetConnectionProvidedName("SimulationRabbitMQ");
                            configurator.UseConnectionString($"amqp://{rabbitMqSettings.Username}:{rabbitMqSettings.Password}@{rabbitMqSettings.Host}:{rabbitMqSettings.Port}/");
                        }));

                s.AddSingleton<IAsyncSubscriber>(
                    sp =>
                    {
                        var bus = sp.GetRequiredService<IBus>();
                        var exchangeName = rabbitMqSettings.SimulationExchangeName ?? throw new MissingConfigurationException(nameof(rabbitMqSettings.SimulationExchangeName));
                        var queueName = rabbitMqSettings.IncomingMessageQueueName ?? throw new MissingConfigurationException(nameof(rabbitMqSettings.IncomingMessageQueueName));
        
                        return bus.CreateAsyncSubscriber(
                            x =>
                                x.SetExchange(exchangeName)
                                    .SetRoutingKey(queueName)
                                    .SetConsumerTag(queueName)
                                    .SetReceiveSelfPublish(false)
                        );
                    });
                
                s.AddSingleton<IMessageSubscriberService, MessageSubscriberService>();
            });
        
        return hostBuilder;
    }

    public static IHostBuilder AddHandlers(this IHostBuilder hostBuilder, Assembly handlerAssembly)
    {
        hostBuilder.ConfigureServices(
            (_, s) =>
            {
                var messageHandlers = DiscoverHandlers(handlerAssembly).ToList();
                messageHandlers.ForEach(
                    x => s.AddScoped(
                        x.GetInterfaces().First(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ISimulationMessageHandler<>)),
                        x));
                
                var queryMessageHandlers = DiscoverQueryHandlers(handlerAssembly).ToList();
                queryMessageHandlers.ForEach(
                    x => s.AddScoped(
                        x.GetInterfaces().First(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ISimulationQueryHandler<,>)),
                        x));
            });
        
        return hostBuilder;
    }

    public static IHostBuilder AddSimulationServices(this IHostBuilder hostBuilder) => hostBuilder.ConfigureContainer<ContainerBuilder>(
        (_, builder) =>
        {
            builder.RegisterModule<ApplicationModule>();
            
            builder.RegisterType<SimulationMessageProcessor>().As<ISimulationMessageProcessor>().SingleInstance();
            builder.RegisterType<MessageService>().As<IMessageService>().SingleInstance();
            
            builder.RegisterType<MapService>().As<IMapService>();
        });

    public static IHostBuilder ConfigureRabbitMq(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices(
            (ctx, s) =>
            {
                var rabbitMqSettings = ctx.Configuration.GetSettings<RabbitMqSettings>(nameof(RabbitMqSettings));
                var factory = new ConnectionFactory
                    { HostName = rabbitMqSettings.Host, Password = rabbitMqSettings.Password, UserName = rabbitMqSettings.Username, Port = rabbitMqSettings.Port };
                using var connection = factory.CreateConnection();
                using var model = connection.CreateModel();

                if (rabbitMqSettings.SimulationExchangeName is not null)
                    model.ExchangeDeclare(rabbitMqSettings.SimulationExchangeName, ExchangeType.Topic, false, true);
            });
        
        return hostBuilder;
    }
    
    public static IHost SubscribeMessageSubscriber(this IHost host, Assembly messageAssembly)
    {
        // var bus = host.Services.GetRequiredService<IBus>();
        // var rabbitMqSettings = host.Services.GetRequiredService<RabbitMqSettings>();
        //
        // var exchangeName = rabbitMqSettings.SimulationExchangeName ?? throw new MissingConfigurationException(nameof(rabbitMqSettings.SimulationExchangeName));
        // var queueName = rabbitMqSettings.IncomingMessageQueueName ?? throw new MissingConfigurationException(nameof(rabbitMqSettings.IncomingMessageQueueName));
        
        // var messageSubscriber = bus.CreateAsyncSubscriber(
        //     x =>
        //         x.SetExchange(exchangeName)
        //             .SetRoutingKey(queueName)
        //             .SetConsumerTag(queueName)
        //             .SetReceiveSelfPublish(false)
        // );
        
        // foreach (var handlerType in DiscoverHandlers(handlerAssembly))
        // {
        //     var messageHandlerInterface = handlerType.GetInterfaces().First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IMessageHandler<>));
        //     var messageType = messageHandlerInterface.GetGenericArguments()[0];
        //     typeof(InfrastructureExtensions).GetMethod(nameof(AddHandler), BindingFlags.NonPublic | BindingFlags.Static)!
        //         .MakeGenericMethod(messageType).Invoke(null, new object[] { messageSubscriber, host.Services });
        // }

        var messageSubscriber = host.Services.GetRequiredService<IAsyncSubscriber>();
        
        foreach (var messageType in DiscoverMessageTypes(new [] { messageAssembly }))
            typeof(InfrastructureExtensions).GetMethod(nameof(SubscribeForMessage), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(messageType).Invoke(null, new object[] { messageSubscriber, host.Services });
        
        foreach (var messageType in DiscoverSimulationQueryTypes(new [] { messageAssembly }))
            typeof(InfrastructureExtensions).GetMethod(nameof(SubscribeForSimulationQueries), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(messageType, messageType.GetInterface(typeof(ISimulationQuery<>).Name)!.GetGenericArguments().First()).Invoke(null, new object[] { messageSubscriber, host.Services });
        
        messageSubscriber.Open();

        return host;
    }
    
    public static IHostBuilder AddSerilog(this IHostBuilder hostBuilder) => hostBuilder
        .ConfigureLogging((_, loggingBuilder) => loggingBuilder.ClearProviders())
        .UseSerilog(
            (ctx, config) =>
            {
                var lokiSettings = ctx.Configuration.GetSettings<LokiSettings>(nameof(LokiSettings));

                var lokiCredentials = new LokiCredentials { Login = lokiSettings.Login, Password = lokiSettings.Password };
                var labels = new[] { new LokiLabel { Key = "Id", Value = lokiSettings.Label } };
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
    
    private static IAsyncSubscriber SubscribeForMessage<TMessage>(this IAsyncSubscriber subscriber, IServiceProvider serviceProvider)
        where TMessage : class, ISimulationMessage
    {
        subscriber.Subscribe<TMessage>(
            async x =>
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                await scope.ServiceProvider.GetRequiredService<IMessageSubscriberService>().ReceiveMessage(x);
            });
        return subscriber;
    }
    
    private static IAsyncSubscriber SubscribeForSimulationQueries<TQuery, TResult>(this IAsyncSubscriber subscriber, IServiceProvider serviceProvider)
        where TQuery : class, ISimulationQuery<TResult>
        where TResult : ISimulationMessage
    {
        subscriber.Subscribe<TQuery, TResult>(
            async x =>
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var simulation = scope.ServiceProvider.GetRequiredService<ISimulation>();
                return await scope.ServiceProvider.GetRequiredService<ISimulationQueryHandler<TQuery, TResult>>().HandleQueryAsync(simulation, x);
            });
        return subscriber;
    }
    
    private static IEnumerable<Type> DiscoverHandlers(Assembly assembly) => assembly.GetTypes().Where(
        x => !x.IsAbstract && x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ISimulationMessageHandler<>)));
    
    private static IEnumerable<Type> DiscoverQueryHandlers(Assembly assembly) => assembly.GetTypes().Where(
        x => !x.IsAbstract && x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ISimulationQueryHandler<,>)));

    private static IEnumerable<Type> DiscoverMessageTypes(IEnumerable<Assembly> assemblies)
    {
        var messageTypes = new List<Type>();
        foreach (var assembly in assemblies)
            messageTypes.AddRange(
                assembly.GetTypes().Where(
                    x => !x.IsAbstract && x.IsAssignableTo(typeof(ISimulationMessage))));
        return messageTypes;
    }
    
    private static IEnumerable<Type> DiscoverSimulationQueryTypes(IEnumerable<Assembly> assemblies)
    {
        var messageTypes = new List<Type>();
        foreach (var assembly in assemblies)
            messageTypes.AddRange(
                assembly.GetTypes().Where(
                    x => !x.IsAbstract && x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ISimulationQuery<>))));
        return messageTypes;
    }
}