using Shared.Application.Services;
using Shared.Domain.DomainEvents;
using Shared.Domain.DomainEvents.Incident;
using Shared.Domain.DomainEvents.Patrol;
using WebApp.Application.Services.Statistics;

namespace WebApp.Application.Services;

internal class LocalDomainEventProcessor : IDomainEventProcessor
{
    private readonly IStatisticsManager _statisticsManager;

    public LocalDomainEventProcessor(IStatisticsManager statisticsManager)
    {
        _statisticsManager = statisticsManager;
    }

    public Task ProcessDomainEvents(IEnumerable<IDomainEvent> domainEvents) => Task.WhenAll(domainEvents.Select(ProcessDomainEventLocally));

    private Task ProcessDomainEventLocally(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case IncidentCreated incidentCreated:
                _statisticsManager.AddIncident(incidentCreated.Id, incidentCreated.Location, DateTimeOffset.UtcNow);
                break;
            case IncidentStatusUpdated incidentStatusUpdated:
                _statisticsManager.UpdateIncident(incidentStatusUpdated.Incident.Id, incidentStatusUpdated.NewStatus, incidentStatusUpdated.Incident.UpdatedAt);
                break;
            case PatrolCreated patrolCreated:
                _statisticsManager.AddPatrol(patrolCreated.PatrolId, patrolCreated.Position);
                break;
            case PatrolStatusUpdated patrolStatusUpdated:
                _statisticsManager.UpdatePatrol(patrolStatusUpdated.Patrol.PatrolId, patrolStatusUpdated.NewStatus, patrolStatusUpdated.Patrol.UpdatedAt);
                break;
            case PatrolPositionUpdated patrolPositionUpdated:
                _statisticsManager.UpdatePatrol(patrolPositionUpdated.Patrol.PatrolId, patrolPositionUpdated.NewPosition, patrolPositionUpdated.Patrol.UpdatedAt);
                break;
        }

        return Task.CompletedTask;
    }
}