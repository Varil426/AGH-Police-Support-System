using Shared.Application.Agents.Communication.Messages;
using Shared.Application.Integration.Commands;
using Shared.Application.Integration.Events;
using Shared.Application.Integration.Queries;

namespace Shared.Application.Services;

public interface IMessageBus
{
    Task InvokeAsync<TCommand>(TCommand command, CancellationToken cancellation = default) where TCommand : ICommand;

    Task<TResult> InvokeAsync<TCommand, TResult>(TCommand command, CancellationToken cancellation = default, TimeSpan? timeout = null)
        where TCommand : class, ICommand<TResult>
        where TResult : class;

    Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellation = default, TimeSpan? timeout = null)
        where TQuery : class, IQuery<TResult>
        where TResult : class;
    
    Task SendAsync<TMessage>(TMessage message) where TMessage : class, IMessage;

    Task PublishAsync<TEvent>(TEvent @event) where TEvent : class, IEvent;
}