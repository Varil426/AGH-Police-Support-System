using HqService.Application.DTOs;
using Shared.Domain.Incident;

namespace HqService.Application.Services;

public interface IIncidentMonitoringService
{
    IReadOnlySet<Incident> OnGoingIncidents { get; }
    
    IReadOnlySet<Incident> Incidents { get; }
        
    void AddIncident(Incident incident);

    Incident? GetIncidentById(Guid id);

    void UpdatedIncident(UpdateIncidentDto updateIncidentDto);
}