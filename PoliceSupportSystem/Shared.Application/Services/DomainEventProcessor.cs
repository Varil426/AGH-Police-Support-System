using Shared.Domain.DomainEvents;

namespace Shared.Application.Services;

internal class DomainEventProcessor : IDomainEventProcessor
{
    private readonly IDomainEventMapper _mapper;
    private readonly IMessageBus _messageBus;

    public DomainEventProcessor(IDomainEventMapper mapper, IMessageBus messageBus)
    {
        _mapper = mapper;
        _messageBus = messageBus;
    }

    public async Task ProcessDomainEvents(IEnumerable<IDomainEvent> domainEvents)
    {
        var events = _mapper.Map(domainEvents).ToList();
        foreach (var @event in events) 
            await _messageBus.PublishAsync(@event);
    }
}