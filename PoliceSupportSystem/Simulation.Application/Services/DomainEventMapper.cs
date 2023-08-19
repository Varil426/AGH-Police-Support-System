using Shared.Domain.DomainEvents;
using Shared.Domain.DomainEvents.Incident;
using Simulation.Shared.Communication;

namespace Simulation.Application.Services;

internal class DomainEventMapper : IDomainEventMapper
{
    public ISimulationMessage Map(IDomainEvent domainEvent) => domainEvent switch
    {
        IncidentCreated incidentCreated => new NewIncidentMessage(incidentCreated.IncidentId, incidentCreated.Location, incidentCreated.Type, incidentCreated.Status),
        _ => throw new Exception($"Cannot map domain event of type {domainEvent.GetType().Name}")
    };
}