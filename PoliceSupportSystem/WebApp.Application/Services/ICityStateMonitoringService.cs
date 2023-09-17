using Shared.Application.Integration.DTOs;
using Shared.Domain.Incident;

namespace WebApp.Application.Services;

public interface ICityStateMonitoringService //: IAsyncSubscribable<ICityStateMonitoringService>
{
    Task AddIncident(NewIncidentDto newIncidentDto);
    // void AddPatrol();
    Task UpdateIncident(UpdateIncidentDto updateIncidentDto);
    // void UpdatePatrol();

    IReadOnlyCollection<Incident> GetIncidents();
}