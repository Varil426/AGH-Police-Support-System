// using MassTransit;

using MassTransit;
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
    // private readonly Wolverine.IMessageBus _messageBus;
    //
    // public MessageBus(Wolverine.IMessageBus messageBus)
    // {
    //     _messageBus = messageBus;
    // }
    //
    // public Task InvokeAsync<TCommand>(TCommand command, CancellationToken cancellation = default(CancellationToken), TimeSpan? timeout = null) where TCommand : ICommand
    //     => _messageBus.InvokeAsync(command, cancellation, timeout);
    //
    // public Task<TResult> InvokeAsync<TCommand, TResult>(TCommand command, CancellationToken cancellation = default(CancellationToken), TimeSpan? timeout = null)
    //     where TCommand : ICommand<TResult>
    //     => _messageBus.InvokeAsync<TResult>(command, cancellation, timeout);
    //
    // public Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellation = default(CancellationToken), TimeSpan? timeout = null)
    //     where TQuery : IQuery<TResult>
    //     => _messageBus.InvokeAsync<TResult>(query, cancellation, timeout);
    //
    // public ValueTask SendAsync<TMessage>(TMessage message) where TMessage : IMessage
    //     => _messageBus.SendAsync(message);
    //
    // public ValueTask PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
    //     => _messageBus.PublishAsync(@event);
    
    // private readonly IBus _messageBus;
    //
    // public MessageBus(IBus messageBus)
    // {
    //     _messageBus = messageBus;
    // }
    //
    // public Task InvokeAsync<TCommand>(TCommand command, CancellationToken cancellation = default) where TCommand : ICommand
    //     => _messageBus.Send(command, cancellation);
    //
    // public async Task<TResult> InvokeAsync<TCommand, TResult>(TCommand command, CancellationToken cancellation = default, TimeSpan? timeout = null)
    //     where TCommand : class, ICommand<TResult>
    //     where TResult : class
    //     => (await _messageBus.Request<TCommand, TResult>(command, cancellation, timeout ?? default)).Message;
    //
    // public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellation = default, TimeSpan? timeout = null)
    //     where TQuery : class, IQuery<TResult>
    //     where TResult : class
    //     => (await _messageBus.Request<TQuery, TResult>(query, cancellation, timeout ?? default)).Message;
    //
    // public Task SendAsync<TMessage>(TMessage message) where TMessage : class, IMessage
    //     => _messageBus.Send(message);
    //
    // public Task PublishAsync<TEvent>(TEvent @event) where TEvent : class, IEvent
    //     => _messageBus.Publish(@event);
    
    private readonly IBus _messageBus;
    private readonly ServiceSettings _serviceSettings;
    private readonly RabbitMqSettings _rabbitMqSettings;
    private readonly IRpcAsyncPublisher? _eventPublisher;
    
    public MessageBus(IBus messageBus, ServiceSettings serviceSettings, RabbitMqSettings rabbitMqSettings)
    {
        _messageBus = messageBus;
        _serviceSettings = serviceSettings;
        _rabbitMqSettings = rabbitMqSettings;
        if (rabbitMqSettings.EventExchange is not null)
            _eventPublisher = _messageBus.CreateAsyncRpcPublisher(
                x =>
                {
                    x.SetConsumerTag(_serviceSettings.Id);
                    x.SetExchange(_rabbitMqSettings.EventExchange);
                });
    }

    public Task InvokeAsync<TCommand>(TCommand command, CancellationToken cancellation = default) where TCommand : ICommand
    {
        using var publisher = _messageBus.CreateAsyncRpcPublisher(
            x =>
            {
                x.SetConsumerTag(_serviceSettings.Id);
                x.SetRoutingKey(command.Receiver);
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
                x.SetConsumerTag(_serviceSettings.Id);
                x.SetRoutingKey(command.Receiver);
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
                x.SetConsumerTag(_serviceSettings.Id);
                x.SetRoutingKey(query.Receiver);
                x.SetExchange(_rabbitMqSettings.QueryExchange ?? throw new MissingConfigurationException(nameof(RabbitMqSettings.QueryExchange)));
            });
        return await publisher.Send<TQuery, TResult>(query, cancellationToken: cancellation);
    }
    
    public Task SendAsync<TMessage>(TMessage message) where TMessage : class, IMessage
        => _eventPublisher.Send(message); // TODO
    
    public Task PublishAsync<TEvent>(TEvent @event) where TEvent : class, IEvent
        => _eventPublisher.Send(@event); // TODO

    public void Dispose() // TODO Improve
    {
        _messageBus.Dispose();
        _eventPublisher?.Dispose();
    }
}