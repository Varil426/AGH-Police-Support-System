using Shared.Application.Integration.DTOs;
using Shared.CommonTypes.Incident;
using Shared.Domain.Incident;

namespace HqService.Application.Services;

public interface IIncidentMonitoringService
{
    async Task<IReadOnlySet<Incident>> GetNotHandledIncidents() => (await GetOnGoingIncidents()).Where(x => x.Status == IncidentStatusEnum.WaitingForResponse).ToHashSet();
    
    Task<IReadOnlySet<Incident>> GetOnGoingIncidents();
    
    Task<IReadOnlySet<Incident>> GetIncidents();
        
    Task AddIncident(Incident incident);

    Task<Incident?> GetIncidentById(Guid id);

    Task UpdatedIncident(UpdateIncidentDto updateIncidentDto);
}