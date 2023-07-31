using Shared.Application.Integration.Events;

namespace Shared.Application.Handlers;

public interface IEventHandler<in TEvent> /*: IConsumer<TEvent>*/ where TEvent : /*class /* class - required by MassTransit #1#,*/ IEvent
{
    Task Handle(TEvent @event);
}