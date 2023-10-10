using Shared.Application.Integration.DTOs;
using Shared.CommonTypes.Geo;
using Shared.Domain.Incident;
using Shared.Domain.Patrol;

namespace WebApp.Application.Services;

public interface ICityStateMonitoringService //: IAsyncSubscribable<ICityStateMonitoringService>
{
    Position HqLocation { get; }
    IReadOnlyCollection<Incident> Incidents { get; }
    IReadOnlyCollection<Patrol> Patrols { get; }
    
    Task AddIncident(NewIncidentDto newIncidentDto);
    Task AddPatrol(NewPatrolDto newPatrolDto);
    Task UpdateIncident(UpdateIncidentDto updateIncidentDto);
    // void UpdatePatrol();

    // IReadOnlyCollection<Incident> GetIncidents();
}