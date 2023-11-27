using MessageBus.Core.API;
using Simulation.Application.Services;
using Simulation.Communication.Messages;
using Simulation.Infrastructure.Exceptions;
using Simulation.Infrastructure.Settings;

namespace Simulation.Infrastructure.Services;

internal class MessageService : IMessageService, IDisposable
{
    private readonly IMessageSubscriberService _messageSubscriberService;
    private readonly IBus _bus;
    private readonly RabbitMqSettings _rabbitMqSettings;
    private readonly IPublisher _messagePublisher;

    public MessageService(IMessageSubscriberService messageSubscriberService, IBus bus, RabbitMqSettings rabbitMqSettings)
    {
        _messageSubscriberService = messageSubscriberService;
        _bus = bus;
        _rabbitMqSettings = rabbitMqSettings;

        _messagePublisher = _bus.CreatePublisher(
            x =>
            {
                x.SetExchange(rabbitMqSettings.SimulationExchangeName ?? throw new MissingConfigurationException(nameof(rabbitMqSettings.SimulationExchangeName)));
                x.SetRoutingKey("public");
            });
    }

    public Task<IEnumerable<ISimulationMessage>> GetMessagesAsync() => _messageSubscriberService.GetUnhandledMessages();

    public async Task SendMessageAsync(ISimulationMessage message, string receiver)
    {
        using var publisher = _bus.CreatePublisher(
            x =>
            {
                x.SetExchange(_rabbitMqSettings.SimulationExchangeName);
                x.SetRoutingKey(receiver);
            });

        publisher.Send(message);
    }

    public Task SendMessageAsync(IDirectSimulationMessage message) => SendMessageAsync(message, message.Receiver);

    public Task PublishMessageAsync(ISimulationMessage message)
    {
        _messagePublisher.Send(message);
        return Task.CompletedTask;
    }

    public async Task SendMessagesAsync(IEnumerable<ISimulationMessage> messages, string receiver)
    {
        using var publisher = _bus.CreatePublisher(
            x =>
            {
                x.SetExchange(_rabbitMqSettings.SimulationExchangeName);
                x.SetRoutingKey(receiver);
            });

        foreach (var message in messages)
            publisher.Send(message);
    }

    public async Task SendMessagesAsync(IEnumerable<IDirectSimulationMessage> messages) => await Task.WhenAll(messages.Select(SendMessageAsync));

    public async Task PublishMessagesAsync(IEnumerable<ISimulationMessage> messages) => await Task.WhenAll(messages.Select(PublishMessageAsync));


    public void Dispose()
    {
        _messagePublisher.Dispose();
    }
}