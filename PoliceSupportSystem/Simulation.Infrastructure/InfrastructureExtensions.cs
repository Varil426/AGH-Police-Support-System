using System.Reflection;
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
using Simulation.Application;
using Simulation.Application.Services;
using Simulation.Infrastructure.Exceptions;
using Simulation.Infrastructure.Services;
using Simulation.Infrastructure.Settings;
using Simulation.Shared.Communication;

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

                serviceCollection.AddSingleton(rabbitMqSettings);
                serviceCollection.AddSingleton(simulationSettings);
            });
        return hostBuilder;
    }

    public static TSettings GetSettings<TSettings>(this IConfiguration configuration, string sectionName)
    {
        var configSection = configuration.GetRequiredSection(sectionName);
        var settings = configSection.Get<TSettings>() ?? throw new Exception(); // TODO Change exception type
        return settings;
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
            });
        
        return hostBuilder;
    }

    public static IHostBuilder AddHandlers(this IHostBuilder hostBuilder, Assembly handlerAssembly)
    {

        hostBuilder.ConfigureServices(
            (_, s) =>
            {
                var messageHandlers = DiscoverHandlers(handlerAssembly).ToList();
                messageHandlers.ForEach(x => s.AddScoped(x));
            });
        
        return hostBuilder;
    }

    public static IHostBuilder AddMessageSubscriber(this IHostBuilder hostBuilder) =>
        hostBuilder.ConfigureServices(
            s => s.AddSingleton<IMessageSubscriberService, MessageSubscriberService>());
    
    public static IHostBuilder AddMessageService(this IHostBuilder hostBuilder) =>
        hostBuilder.ConfigureServices(
            s => s.AddSingleton<IMessageService, MessageService>());

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
                var labels = new[] { new LokiLabel { Key = nameof(lokiSettings.Label), Value = lokiSettings.Label } };
                var propertiesAsLabels = new[] { nameof(lokiSettings.Label) }; // TODO Is it needed?
            
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
    
    private static IEnumerable<Type> DiscoverHandlers(Assembly assembly) => assembly.GetTypes().Where(
        x => x.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IMessageHandler<>)));

    private static IEnumerable<Type> DiscoverMessageTypes(IEnumerable<Assembly> assemblies)
    {
        var messageTypes = new List<Type>();
        foreach (var assembly in assemblies)
            messageTypes.AddRange(
                assembly.GetTypes().Where(
                    x => !x.IsAbstract && x.IsAssignableTo(typeof(ISimulationMessage))));
        return messageTypes;
    }
}