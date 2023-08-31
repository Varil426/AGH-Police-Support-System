using Shared.Application.Integration.Events;
using Shared.Domain.DomainEvents;
using Shared.Domain.DomainEvents.Incident;

namespace Shared.Application.Services;

internal class DomainEventMapper : IDomainEventMapper
{
    public IEvent Map(IDomainEvent @event) => @event switch
    {
        IncidentCreated e => new IncidentCreatedEvent(e.IncidentId, e.Location, e.Status, e.Type),
        _ => throw new Exception($"Event type ({@event.GetType().Name}) not supported")
    };

    public IEnumerable<IEvent> Map(IEnumerable<IDomainEvent> events) => events.Select(Map);
}