using MessageBus.Core.API;
using Shared.Application;
using Shared.Application.Agents.Communication.Messages;
using Shared.Application.Integration.Commands;
using Shared.Application.Integration.Events;
using Shared.Application.Integration.Queries;
using Shared.Infrastructure.Exceptions;
using Shared.Infrastructure.Settings;
using IBus = MessageBus.Core.API.IBus;

namespace Shared.Infrastructure.Services;

internal class MessageBus : IMessageBus, IDisposable
{
    private readonly IBus _messageBus;
    private readonly ServiceSettings _serviceSettings;
    private readonly RabbitMqSettings _rabbitMqSettings;
    private readonly ThreadLocal<IPublisher>? _eventPublisher;
    private readonly ThreadLocal<IPublisher>? _messagePublisher;
    
    public MessageBus(IBus messageBus, ServiceSettings serviceSettings, RabbitMqSettings rabbitMqSettings)
    {
        _messageBus = messageBus;
        _serviceSettings = serviceSettings;
        _rabbitMqSettings = rabbitMqSettings;
        if (rabbitMqSettings.EventExchange is not null)
            _eventPublisher = new ThreadLocal<IPublisher>(
                () => _messageBus.CreatePublisher(
                    x =>
                    {
                        x.SetExchange(_rabbitMqSettings.EventExchange);
                    }));
        if (rabbitMqSettings.MessageExchange is not null)
            _messagePublisher = new ThreadLocal<IPublisher>(() => _messageBus.CreatePublisher(
                x =>
                {
                    x.SetExchange(_rabbitMqSettings.MessageExchange);
                }));
    }

    public Task InvokeAsync<TCommand>(TCommand command, CancellationToken cancellation = default) where TCommand : ICommand
    {
        using var publisher = _messageBus.CreateAsyncRpcPublisher(
            x =>
            {
                x.SetRoutingKey(command.Receiver);
                x.SetConsumerTag(_serviceSettings.Id);
                x.SetExchange(_rabbitMqSettings.CommandExchange ?? throw new MissingConfigurationException(nameof(RabbitMqSettings.CommandExchange)));
            });
        return publisher.Send(command, cancellationToken: cancellation);
    }
    
    public async Task<TResult> InvokeAsync<TCommand, TResult>(TCommand command, CancellationToken cancellation = default, TimeSpan? timeout = null)
        where TCommand : class, ICommand<TResult>
        where TResult : class
    {
        using var publisher = _messageBus.CreateAsyncRpcPublisher(
            x =>
            {
                x.SetRoutingKey(command.Receiver);
                x.SetConsumerTag(_serviceSettings.Id);
                x.SetExchange(_rabbitMqSettings.CommandExchange ?? throw new MissingConfigurationException(nameof(RabbitMqSettings.CommandExchange)));
            });
        return await publisher.Send<TCommand, TResult>(command, cancellationToken: cancellation);
    }

    public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellation = default, TimeSpan? timeout = null)
        where TQuery : class, IQuery<TResult>
        where TResult : class
    {
        using var publisher = _messageBus.CreateAsyncRpcPublisher(
            x =>
            {
                x.SetRoutingKey(query.Receiver);
                x.SetConsumerTag(_serviceSettings.Id);
                x.SetExchange(_rabbitMqSettings.QueryExchange ?? throw new MissingConfigurationException(nameof(RabbitMqSettings.QueryExchange)));
            });
        return await publisher.Send<TQuery, TResult>(query, cancellationToken: cancellation);
    }

    public Task SendAsync<TMessage>(TMessage message) where TMessage : class, IMessage
    {
        if (message.Receivers != null && message.Receivers.Any())
        {
            foreach (var receiver in message.Receivers)
            {
                using var publisher = _messageBus.CreatePublisher(
                    x =>
                    {
                        x.SetRoutingKey(receiver.ToString());
                        x.SetExchange(_rabbitMqSettings.DirectMessageExchange ?? throw new MissingConfigurationException(nameof(RabbitMqSettings.DirectMessageExchange)));
                    });
                publisher.Send(message);
            }
        }
        else if (_messagePublisher is not null)
            _messagePublisher.Value?.Send(message);
        else
            throw new InvalidOperationException("Missing config for messages.");
        
        return Task.CompletedTask;
    }
    
    public Task PublishAsync<TEvent>(TEvent @event) where TEvent : class, IEvent
    {
        if (_eventPublisher is not null)
            _eventPublisher.Value?.Send(@event);
        else 
            throw new InvalidOperationException("Missing config for events.");
        return Task.CompletedTask;
    }

    public void Dispose() // TODO Improve
    {
        _messageBus.Dispose();
        _eventPublisher?.Dispose();
        _messagePublisher?.Dispose();
    }
}