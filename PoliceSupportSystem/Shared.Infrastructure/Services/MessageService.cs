using System.Reflection;
using Shared.Application.Agents;
using Shared.Application.Handlers;
using Shared.Application.Services;
using Shared.Infrastructure.Exceptions;
using Shared.Infrastructure.Settings;
using IMessage = Shared.Application.Agents.Communication.Messages.IMessage;

namespace Shared.Infrastructure.Services;

internal class MessageService : IMessageService, IMessageHandler, IDisposable
{
    private readonly IMessageBus _messageBus;
    private readonly IBusSubscriberManager _subscriberManager;
    private readonly RabbitMqSettings _rabbitMqSettings;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISet<IAgent> _subscribedAgents = new HashSet<IAgent>();
    private readonly SemaphoreSlim _subscribeSemaphore = new(1,1);

    public MessageService(IMessageBus messageBus, IBusSubscriberManager subscriberManager, RabbitMqSettings rabbitMqSettings, IServiceProvider serviceProvider)
    {
        _messageBus = messageBus;
        _subscriberManager = subscriberManager;
        _rabbitMqSettings = rabbitMqSettings;
        _serviceProvider = serviceProvider;
    }

    public Task Handle(IMessage message)
    {
        Console.WriteLine($"Received message: {message.MessageType}"); // TODO Remove
        var validReceivers = _subscribedAgents.Where(x => x.AcceptedMessageTypes.Any(type => type.IsInstanceOfType(message)));
        return Task.WhenAll(validReceivers.SkipWhile(x => message.Receivers != null && !message.Receivers.Contains(x.Id)).Select(x => x.PushMessageAsync(message)));
    }

    public async Task SendMessageAsync(IMessage message) => await _messageBus.SendAsync(message);

    public async Task SubscribeForMessagesAsync(IAgent agent)
    {
        await _subscribeSemaphore.WaitAsync();
        _subscribedAgents.Add(agent);
        SubscribeForDirectMessages(agent);
        _subscribeSemaphore.Release();
    }
    
    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void SubscribeForDirectMessages(IAgent agent)
    {
        var messageSubscriber = _subscriberManager.CreateSubscriber(
            x =>
                x.SetExchange(_rabbitMqSettings.DirectMessageExchange ?? throw new MissingConfigurationException(nameof(_rabbitMqSettings.DirectMessageExchange)))
                    .SetRoutingKey(agent.Id.ToString())
                    .SetConsumerTag(agent.Id.ToString())
                    .SetReceiveSelfPublish() // TODO Add to config
        );

        foreach (var messageType in Extensions.DiscoverMessageTypes(new[] { typeof(IMessage).Assembly }))
            typeof(Extensions).GetMethod(nameof(Extensions.AddMessageHandler), BindingFlags.Static | BindingFlags.NonPublic)!.MakeGenericMethod(messageType)
                .Invoke(null, new object[] { messageSubscriber, _serviceProvider });

        messageSubscriber.Open();
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            _subscribeSemaphore.Dispose();
        }
    }
}