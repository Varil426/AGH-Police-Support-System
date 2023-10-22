using System.Reflection;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<MessageService> _logger;
    private readonly ISet<IAgent> _subscribedAgents = new HashSet<IAgent>();
    private readonly SemaphoreSlim _subscribeSemaphore = new(1,1);

    public MessageService(
        IMessageBus messageBus,
        IBusSubscriberManager subscriberManager,
        RabbitMqSettings rabbitMqSettings,
        IServiceProvider serviceProvider,
        ILogger<MessageService> logger)
    {
        _messageBus = messageBus;
        _subscriberManager = subscriberManager;
        _rabbitMqSettings = rabbitMqSettings;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task Handle(IMessage message)
    {
        _logger.LogInformation($"Received the message: {message.MessageType} with ID: {message.MessageId}.");
        var validReceivers = _subscribedAgents.Where(x => x.AcceptedMessageTypes.Any(type => type.IsInstanceOfType(message))).ToList();
        if (!validReceivers.Any())
        {
            _logger.LogInformation($"No valid receivers for the message: {message.MessageType} with ID: {message.MessageId}.");
            return;
        }
        await Task.WhenAll(validReceivers.SkipWhile(x => message.Receivers != null && !message.Receivers.Contains(x.Id)).Select(x => x.PushMessageAsync(message)));
        _logger.LogInformation("Successfully pushed the message with ID: {id}.", message.MessageId);
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