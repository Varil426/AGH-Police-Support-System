using Riok.Mapperly.Abstractions;
using Shared.Domain.DomainEvents;
using Shared.Domain.DomainEvents.Incident;
using Simulation.Shared.Communication;

namespace Simulation.Application.Services;

[Mapper]
internal partial class DomainEventMapper : IDomainEventMapper
{
    public ISimulationMessage Map(IDomainEvent domainEvent) => domainEvent switch
    {
        IncidentCreated incidentCreated => Map(incidentCreated),
        _ => throw new Exception($"Cannot map domain event of type {domainEvent.GetType().Name}")
    };

    private partial NewIncidentMessage Map(IncidentCreated incidentCreated);
}