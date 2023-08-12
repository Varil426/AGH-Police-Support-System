using HqService.Application.DTOs;
using Shared.Application.Helpers;
using Shared.Domain.Incident;

namespace HqService.Application.Services;

public class IncidentMonitoringService : IIncidentMonitoringService
{
    private Dictionary<Guid, Incident> _incidents = new();

    public IReadOnlySet<Incident> OnGoingIncidents => _incidents.Values.Where(x => x.Status != IncidentStatusEnum.Resolved).ToHashSet();
    
    public IReadOnlySet<Incident> Incidents =>  _incidents.Values.ToHashSet();

    public IncidentMonitoringService()
    {
    }

    public void AddIncident(Incident incident) => _incidents.Add(incident.Id, incident);

    public Incident? GetIncidentById(Guid id) => _incidents.TryGet(id);
    
    public void UpdatedIncident(UpdateIncidentDto updateIncidentDto)
    {
        var incident = GetIncidentById(updateIncidentDto.Id) ?? throw new Exception($"Incident with id: {updateIncidentDto.Id} not found");
        incident.Status = updateIncidentDto.NewIncidentStatus;
        incident.Location = updateIncidentDto.NewLocation ?? incident.Location;
        incident.Type = updateIncidentDto.NewIncidentType;
    }
}