using Shared.Application.Factories;
using Shared.Application.Helpers;
using Shared.Application.Integration.DTOs;
using Shared.CommonTypes.Geo;
using Shared.Domain.Incident;
using WebApp.Application.Settings;

namespace WebApp.Application.Services;

internal class CityStateMonitoringService : ICityStateMonitoringService
{
    private readonly IIncidentFactory _incidentFactory;

    private readonly MapSettings _mapSettings;

    // private readonly List<Func<ICityStateMonitoringService, Task>> _subscriptions = new();
    private readonly List<Incident> _incidents = new();

    public Position HqLocation => _mapSettings.HqLocation; 

    public CityStateMonitoringService(IIncidentFactory incidentFactory, MapSettings mapSettings)
    {
        _incidentFactory = incidentFactory;
        _mapSettings = mapSettings;
    }

    // public void Subscribe(Func<ICityStateMonitoringService, Task> callback) => _subscriptions.Add(callback);

    public async Task AddIncident(NewIncidentDto newIncidentDto)
    {
        if (_incidents.Any(x => x.Id == newIncidentDto.Id))
            throw new Exception($"Incident with {newIncidentDto.Id} is already present.");

        _incidents.Add(_incidentFactory.CreateIncident(newIncidentDto));
        // await NotifyAll();
    }

    public async Task UpdateIncident(UpdateIncidentDto updateIncidentDto)
    {
        var incident = _incidents.FirstOrDefault(x => x.Id == updateIncidentDto.Id) ?? throw new Exception($"Incident with ID: {updateIncidentDto.Id} not found");
        incident.Update(updateIncidentDto);
        // await NotifyAll();
    }

    public IReadOnlyCollection<Incident> GetIncidents() => _incidents.ToList().AsReadOnly();

    // private async Task NotifyAll()
    // {
    //     foreach (var subscription in _subscriptions)
    //         await subscription.Invoke(this);
    // }
}