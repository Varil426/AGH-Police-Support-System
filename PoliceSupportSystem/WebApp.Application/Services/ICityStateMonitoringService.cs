using Shared.Application.Integration.DTOs;
using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Incident;
using Shared.Domain.Incident;
using Shared.Domain.Patrol;

namespace WebApp.Application.Services;

public interface ICityStateMonitoringService //: IAsyncSubscribable<ICityStateMonitoringService>
{
    Position HqLocation { get; }
    IReadOnlyCollection<Incident> Incidents { get; }
    IReadOnlyCollection<Incident> ActiveIncidents => Incidents.Where(x => x.Status != IncidentStatusEnum.Resolved).ToList().AsReadOnly();
    IReadOnlyCollection<Patrol> Patrols { get; }
    
    Task AddIncident(NewIncidentDto newIncidentDto);
    Task AddPatrol(NewPatrolDto newPatrolDto);
    Task UpdateIncident(IncidentDto incidentDto);
    Task UpdatePatrol(PatrolDto patrolDto);

    // IReadOnlyCollection<Incident> GetIncidents();
}