using Shared.Application.Integration.DTOs;
using Shared.CommonTypes.Geo;
using Shared.Domain.Incident;

namespace WebApp.Application.Services;

public interface ICityStateMonitoringService //: IAsyncSubscribable<ICityStateMonitoringService>
{
    Position HqLocation { get; }
    
    Task AddIncident(NewIncidentDto newIncidentDto);
    // void AddPatrol();
    Task UpdateIncident(UpdateIncidentDto updateIncidentDto);
    // void UpdatePatrol();

    IReadOnlyCollection<Incident> GetIncidents();
}