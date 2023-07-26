using MessageBus.Core.API;
using Simulation.Application.Services;
using Simulation.Infrastructure.Exceptions;
using Simulation.Infrastructure.Settings;
using Simulation.Shared.Communication;

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
            });
    }

    public Task<IEnumerable<ISimulationMessage>> GetMessagesAsync() => _messageSubscriberService.GetUnhandledMessages();

    public Task SendMessageAsync(ISimulationMessage message, string receiver)
    {
        using var publisher = _bus.CreateAsyncRpcPublisher(
            x =>
            {
                x.SetExchange(_rabbitMqSettings.SimulationExchangeName);
                x.SetRoutingKey(receiver);
            });
        
        return publisher.Send(message);
    }

    public Task SendMessageAsync(IDirectSimulationMessage message) => SendMessageAsync(message, message.Receiver);

    public Task PublishMessageAsync(ISimulationMessage message)
    {
        _messagePublisher.Send(message);
        return Task.CompletedTask;
    }

    public async Task SendMessagesAsync(IEnumerable<ISimulationMessage> messages, string receiver)
    {
        using var publisher = _bus.CreateAsyncRpcPublisher(
            x =>
            {
                x.SetExchange(_rabbitMqSettings.SimulationExchangeName);
                x.SetRoutingKey(receiver);
            });

        foreach (var message in messages) 
            await publisher.Send(message);
    }

    public async Task SendMessagesAsync(IEnumerable<IDirectSimulationMessage> messages) => await Task.WhenAll(messages.Select(SendMessageAsync));

    public async Task PublishMessagesAsync(IEnumerable<ISimulationMessage> messages) => await Task.WhenAll(messages.Select(PublishMessageAsync));


    public void Dispose() // TODO Improve
    {
        _bus.Dispose();
        _messagePublisher.Dispose();
    }
}