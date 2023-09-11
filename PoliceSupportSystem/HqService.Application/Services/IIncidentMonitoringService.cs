using Shared.Application.Integration.DTOs;
using Shared.Domain.Incident;

namespace HqService.Application.Services;

public interface IIncidentMonitoringService
{
    Task<IReadOnlySet<Incident>> GetOnGoingIncidents();
    
    Task<IReadOnlySet<Incident>> GetIncidents();
        
    Task AddIncident(Incident incident);

    Task<Incident?> GetIncidentById(Guid id);

    Task UpdatedIncident(UpdateIncidentDto updateIncidentDto);
}