using System.Collections;
using Riok.Mapperly.Abstractions;
using Shared.CommonTypes.Patrol;
using Shared.Domain.DomainEvents;
using Shared.Domain.DomainEvents.Incident;
using Shared.Domain.DomainEvents.Patrol;
using Simulation.Application.DomainEvents;
using Simulation.Application.Entities;
using Simulation.Application.Entities.Patrol;
using Simulation.Communication.Common;
using Simulation.Communication.Messages;

namespace Simulation.Application.Services;

[Mapper]
internal partial class DomainEventMapper : IDomainEventMapper
{
    private static IEnumerable<ISimulationMessage> Encapsulate(ISimulationMessage simulationMessage) => new[] { simulationMessage };
    private static IEnumerable<ISimulationMessage> Empty() => Enumerable.Empty<ISimulationMessage>();

    public IEnumerable<ISimulationMessage> Map(IDomainEvent domainEvent) => domainEvent switch
    {
        // Auto
        IncidentCreated incidentCreated => Encapsulate(Map(incidentCreated)),
        // Custom
        PatrolRelatedServiceAdded relatedServiceAdded => Map(relatedServiceAdded),
        PatrolPositionUpdated patrolPositionUpdated => Map(patrolPositionUpdated),
        IncidentStatusUpdated incidentStatusUpdated => Map(incidentStatusUpdated),
        PatrolStatusUpdated patrolStatusUpdated => Map(patrolStatusUpdated),
        // Skippable
        PatrolCreated => Empty(),
        PatrolActionChanged => Empty(),
        PatrolOrderChanged => Empty(),
        // Not handled
        _ => throw new Exception($"Cannot map domain event of type {domainEvent.GetType().Name}")
    };


    private partial NewIncidentMessage Map(IncidentCreated incidentCreated);

    private IEnumerable<ISimulationMessage> Map(PatrolStatusUpdated patrolStatusUpdated) =>
        patrolStatusUpdated.NewStatus == PatrolStatusEnum.InShooting &&
        (patrolStatusUpdated.Patrol as ISimulationPatrol)?.GetRelatedServicesOfType(ServiceTypeEnum.GunService).First() is { } gunService
            ? Encapsulate(new GunFiredMessage(gunService.Id))
            : Empty();

    private IEnumerable<ISimulationMessage> Map(IncidentStatusUpdated incidentStatusUpdated) => Encapsulate(
        new IncidentStatusUpdatedMessage(Guid.NewGuid(), incidentStatusUpdated.Incident.Id, incidentStatusUpdated.NewStatus, DateTimeOffset.UtcNow));

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