using MassTransit;
using Shared.Application;
using Shared.Application.Integration.Events;

namespace Shared.Infrastructure.Consumers;

public sealed class GenericEventConsumer<TEvent> : IConsumer<TEvent> where TEvent : class, IEvent
{
    private readonly IEventHandler<TEvent> _handler;

    public GenericEventConsumer(IEventHandler<TEvent> handler)
    {
        _handler = handler;
    }

    public Task Consume(ConsumeContext<TEvent> context) => _handler.Handle(context.Message);

}