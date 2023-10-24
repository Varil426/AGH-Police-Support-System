using Riok.Mapperly.Abstractions;
using Shared.Domain.DomainEvents;
using Shared.Domain.DomainEvents.Incident;
using Shared.Domain.DomainEvents.Patrol;
using Simulation.Application.DomainEvents;
using Simulation.Application.Entities.Patrol;
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
        PatrolPositionUpdated patrolPositionUpdated => Map(patrolPositionUpdated),
        // Skippable
        PatrolStatusUpdated patrolStatusUpdated => Empty(),
        PatrolCreated => Empty(),
        PatrolActionChanged => Empty(),
        PatrolOrderChanged => Empty(),
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

    private IEnumerable<PatrolPositionChangedMessage> Map(PatrolPositionUpdated patrolPositionUpdated)
    {
        var simulationPatrol = (SimulationPatrol)patrolPositionUpdated.Patrol;
        var navigationServices = simulationPatrol.GetRelatedServicesOfType(ServiceTypeEnum.NavigationService);
        return navigationServices.Select(x => new PatrolPositionChangedMessage(simulationPatrol.Position, x.Id));
    }
}