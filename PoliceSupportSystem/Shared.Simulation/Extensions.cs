using System.Reflection;
using Autofac;
using Autofac.Features.AttributeFilters;
using MessageBus.Core;
using MessageBus.Core.API;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Shared.Application.Services;
using Shared.Simulation.Decorators;
using Shared.Simulation.Handlers;
using Shared.Simulation.Services;
using Shared.Simulation.Settings;
using Simulation.Shared.Communication;

namespace Shared.Simulation;

public static class Extensions
{
    public static IHostBuilder AddSharedSimulation(this IHostBuilder hostBuilder, Assembly[] handlersAssemblies) => hostBuilder
        .AddRabbitMqSimulationBus()
        .AddInternalServices()
        .AddSimulatedServices()
        .AddSimulationMessageHandlers(handlersAssemblies.Append(typeof(Extensions).Assembly).ToArray())
        .Decorate();

    public static IHostBuilder AddPatrolSpecificSimulationServices(this IHostBuilder hostBuilder) => hostBuilder
        .AddPatrolSettings()
        .AddPatrolServices();

    public static IHost SubscribeSimulationMessageHandlers(this IHost host, Assembly[] handlerAssemblies)
    {
        handlerAssemblies = handlerAssemblies.Append(typeof(Extensions).Assembly).ToArray();
        
        var subscriberManager = host.Services.GetRequiredService<ISimulationBusSubscriberManager>();
        var serviceInfo = host.Services.GetRequiredService<IServiceInfoService>();
        var settings = host.Services.GetRequiredService<SimulationCommunicationSettings>();

        var simulationDirectMessageSubscriber = subscriberManager.CreateSubscriber(
            x =>
            
                x.SetExchange(settings.SimulationExchangeName)
                    .SetRoutingKey(serviceInfo.Id)
                    .SetConsumerTag(serviceInfo.Id)
                    .SetReceiveSelfPublish(false) // TODO Add to config
        );
        
        var simulationMessageSubscriber = subscriberManager.CreateSubscriber(
            x =>
            
                x.SetExchange(settings.SimulationExchangeName)
                    .SetRoutingKey("#")
                    .SetConsumerTag(serviceInfo.Id)
                    .SetReceiveSelfPublish(false) // TODO Add to config
        );
        
        foreach (var handlerType in DiscoverSimulationMessageHandlers(handlerAssemblies))
        {
            var simulationMessageHandlerInterface = handlerType.GetInterfaces().First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ISimulationMessageHandler<>));
            var messageType = simulationMessageHandlerInterface.GetGenericArguments()[0];
            // TODO Can be improved - multiple handlers with Autofac
            typeof(Extensions).GetMethod(nameof(AddSimulationMessageHandler), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(simulationMessageHandlerInterface, messageType).Invoke(null, new object[] { simulationDirectMessageSubscriber, host.Services });
            typeof(Extensions).GetMethod(nameof(AddSimulationMessageHandler), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(simulationMessageHandlerInterface, messageType).Invoke(null, new object[] { simulationMessageSubscriber, host.Services });
        }

        simulationDirectMessageSubscriber.Open();
        simulationMessageSubscriber.Open();
        
        return host;
    }

    private static IAsyncSubscriber AddSimulationMessageHandler<THandler, TMessage>(this IAsyncSubscriber subscriber, IServiceProvider serviceProvider)
        where THandler : class, ISimulationMessageHandler<TMessage>
        where TMessage : class, ISimulationMessage
    {
        subscriber.Subscribe<TMessage>(
            async x =>
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                await scope.ServiceProvider.GetRequiredService<THandler>().Handle(x);
            });
        return subscriber;
    }
    
    private static IHostBuilder AddSimulationMessageHandlers(this IHostBuilder hostBuilder, Assembly[] handlersAssemblies)
    {
        hostBuilder.ConfigureServices(
            s =>
            {
                var messageHandlers = DiscoverSimulationMessageHandlers(handlersAssemblies).ToList();
                messageHandlers.ForEach(
                    x => s.AddScoped(
                        x.GetInterfaces().First(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ISimulationMessageHandler<>)),
                        x));
            });
        
        return hostBuilder;
    }

    private static IHostBuilder Decorate(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureContainer<ContainerBuilder>(
            (ctx, builder) =>
            {
                builder.RegisterGenericDecorator(typeof(DirectSimulationMessageHandlerDecorator<>), typeof(ISimulationMessageHandler<>));
            });
        
        return hostBuilder;
    }
    
    private static IHostBuilder AddRabbitMqSimulationBus(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureRabbitMq();
        
        hostBuilder.ConfigureContainer<ContainerBuilder>(
            (ctx, builder) =>
            {
                var simulationCommunicationSettings = ctx.Configuration.GetSettings<SimulationCommunicationSettings>(nameof(SimulationCommunicationSettings));

                builder.RegisterType<SimulationSubscriberErrorService>().As<IErrorSubscriber>().Keyed<IErrorSubscriber>(Constants.SimulationSubscriberErrorServiceKey)
                    .PreserveExistingDefaults();

                builder.Register(
                    componentContext => new RabbitMQBus(
                        configurator =>
                        {
                            configurator.SetConnectionProvidedName("SimulationRabbitMQ");
                            configurator.UseConnectionString(
                                $"amqp://{simulationCommunicationSettings.Username}:{simulationCommunicationSettings.Password}@{simulationCommunicationSettings.Host}:{simulationCommunicationSettings.Port}/");
                            configurator.UseErrorSubscriber(componentContext.ResolveKeyed<IErrorSubscriber>(Constants.SimulationSubscriberErrorServiceKey));
                        })).As<IBus>().Keyed<IBus>(Constants.SimulationBusKey).SingleInstance().PreserveExistingDefaults();
                
                // builder.Register(
                //     _ => new RabbitMQBus(
                //         configurator =>
                //         {
                //             configurator.UseConnectionString(
                //                 $"amqp://{simulationCommunicationSettings.Username}:{simulationCommunicationSettings.Password}@{simulationCommunicationSettings.Host}:{simulationCommunicationSettings.Port}/");
                //         })).As<IBus>().Keyed<IBus>(Constants.SimulationBusKey);
            });

        return hostBuilder;
    }
    
    private static IHostBuilder ConfigureRabbitMq(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices(
            (ctx, s) =>
            {
                var simulationCommunicationSettings = ctx.Configuration.GetSettings<SimulationCommunicationSettings>(nameof(SimulationCommunicationSettings));
                var factory = new ConnectionFactory
                    {
                        HostName = simulationCommunicationSettings.Host,
                        Password = simulationCommunicationSettings.Password,
                        UserName = simulationCommunicationSettings.Username,
                        Port = simulationCommunicationSettings.Port
                    };
                using var connection = factory.CreateConnection();
                using var model = connection.CreateModel();

                model.ExchangeDeclare(simulationCommunicationSettings.SimulationExchangeName, ExchangeType.Topic, false, true);
            });
        
        return hostBuilder;
    }
    
    private static IHostBuilder AddPatrolSettings(this IHostBuilder builder)
    {
        builder.ConfigureServices(
            (context, services) =>
            {
                var patrolSettings = context.Configuration.GetSettings<PatrolSettings>(nameof(PatrolSettings));
                services.AddSingleton(patrolSettings);
                services.AddSingleton<IPatrolInfoService>(patrolSettings);
            });
    
        return builder;
    }
    
    private static IHostBuilder AddInternalServices(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices(
            (ctx, s) =>
            {
                s.AddSingleton(_ => ctx.Configuration.GetSettings<SimulationCommunicationSettings>(nameof(SimulationCommunicationSettings)));
                s.AddTransient<ITypeMapper, TypeMapper>();
            });
        
        hostBuilder.ConfigureContainer<ContainerBuilder>(
            (ctx, builder) =>
            {
                builder.RegisterType<SimulationMessageBus>().As<ISimulationMessageBus>().WithAttributeFiltering().SingleInstance();
                builder.RegisterType<SimulationBusSubscriberManager>().As<ISimulationBusSubscriberManager>().WithAttributeFiltering().SingleInstance();
            });

        return hostBuilder;
    }
    
    private static IHostBuilder AddPatrolServices(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices(
            (ctx, s) =>
            {
                s.AddScoped<IStatusService, PatrolSimulationStatusService>();
            });
        
        return hostBuilder;
    }
    
    private static IHostBuilder AddSimulatedServices(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices(
            (ctx, s) =>
            {
                s.AddScoped<IStatusService, SimulationStatusService>();
            });
        
        hostBuilder.ConfigureContainer<ContainerBuilder>(
            (ctx, builder) =>
            {
                
            });

        return hostBuilder;
    }
    
    private static TSettings GetSettings<TSettings>(this IConfiguration configuration, string sectionName)
    {
        var configSection = configuration.GetRequiredSection(sectionName);
        var settings = configSection.Get<TSettings>() ?? throw new Exception(); // TODO Change exception type
        return settings;
    }

    private static IEnumerable<Type> DiscoverSimulationMessageHandlers(IEnumerable<Assembly> assemblies)
    {
        var simulationMessageHandlers = new List<Type>();
        foreach (var assembly in assemblies)
            simulationMessageHandlers.AddRange(
                assembly.GetTypes().Where(
                    x => x.GetCustomAttribute<DecoratorAttribute>() is null && x.GetInterfaces().Any(
                        @interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ISimulationMessageHandler<>))));
        return simulationMessageHandlers;
    }
}