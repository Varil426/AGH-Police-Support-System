using Riok.Mapperly.Abstractions;
using Shared.Application.Integration.DTOs;
using Shared.Application.Integration.Events;
using Shared.Domain.DomainEvents;
using Shared.Domain.DomainEvents.Incident;

namespace Shared.Application.Services;

[Mapper]
internal partial class DomainEventMapper : IDomainEventMapper
{
    public IEvent Map(IDomainEvent @event) => @event switch
    {
        IncidentCreated e => new IncidentCreatedEvent(Map(e)),
        _ => throw new Exception($"Event type ({@event.GetType().Name}) not supported")
    };

    public IEnumerable<IEvent> Map(IEnumerable<IDomainEvent> events) => events.Select(Map);

    private partial NewIncidentDto Map(IncidentCreated incidentCreated);
}