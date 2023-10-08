using Riok.Mapperly.Abstractions;
using Shared.Domain.DomainEvents;
using Shared.Domain.DomainEvents.Incident;
using Simulation.Application.DomainEvents;
using Simulation.Communication.Common;
using Simulation.Communication.Messages;

namespace Simulation.Application.Services;

[Mapper]
internal partial class DomainEventMapper : IDomainEventMapper
{
    public IEnumerable<ISimulationMessage> Map(IDomainEvent domainEvent) => domainEvent switch
    {
        // Auto
        IncidentCreated incidentCreated => Encapsulate(Map(incidentCreated)),
        // Custom
        PatrolRelatedServiceAdded relatedServiceAdded => Map(relatedServiceAdded),
        // Skippable
        PatrolCreated patrolCreated => Empty(),
        // Not handled
        _ => throw new Exception($"Cannot map domain event of type {domainEvent.GetType().Name}")
    };

    private IEnumerable<ISimulationMessage> Encapsulate(ISimulationMessage simulationMessage) => new[] { simulationMessage };
    private IEnumerable<ISimulationMessage> Empty() => Enumerable.Empty<ISimulationMessage>();
        
    private partial NewIncidentMessage Map(IncidentCreated incidentCreated);

    private IEnumerable<ISimulationMessage> Map(PatrolRelatedServiceAdded relatedServiceAdded) => relatedServiceAdded.NewService.ServiceType == ServiceTypeEnum.NavigationService
        ? Encapsulate(
            new CurrentPositionMessage(
                relatedServiceAdded.NewService.Id,
                relatedServiceAdded.Patrol.Position))
        : Empty();
}