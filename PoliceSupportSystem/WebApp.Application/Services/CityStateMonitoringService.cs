using Shared.Application.Factories;
using Shared.Application.Helpers;
using Shared.Application.Integration.DTOs;
using Shared.CommonTypes.Geo;
using Shared.Domain.Incident;
using Shared.Domain.Patrol;
using WebApp.Application.Settings;

namespace WebApp.Application.Services;

internal class CityStateMonitoringService : ICityStateMonitoringService
{
    private readonly IIncidentFactory _incidentFactory;

    private readonly MapSettings _mapSettings;
    private readonly IPatrolFactory _patrolFactory;

    // private readonly List<Func<ICityStateMonitoringService, Task>> _subscriptions = new();
    private readonly List<Incident> _incidents = new();
    private readonly List<Patrol> _patrols = new();
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    public Position HqLocation => _mapSettings.HqLocation; 
    public IReadOnlyCollection<Incident> Incidents => _incidents.ToList().AsReadOnly();
    public IReadOnlyCollection<Patrol> Patrols => _patrols.ToList().AsReadOnly();

    public CityStateMonitoringService(IIncidentFactory incidentFactory, MapSettings mapSettings, IPatrolFactory patrolFactory)
    {
        _incidentFactory = incidentFactory;
        _mapSettings = mapSettings;
        _patrolFactory = patrolFactory;
    }

    // public void Subscribe(Func<ICityStateMonitoringService, Task> callback) => _subscriptions.Add(callback);

    public async Task AddIncident(NewIncidentDto newIncidentDto)
    {
        await _semaphoreSlim.WaitAsync();
        if (_incidents.Any(x => x.Id == newIncidentDto.Id))
            throw new Exception($"Incident with {newIncidentDto.Id} is already present.");

        _incidents.Add(_incidentFactory.CreateIncident(newIncidentDto));
        _semaphoreSlim.Release();
    }

    public async Task AddPatrol(NewPatrolDto patrolDto)
    {
        await _semaphoreSlim.WaitAsync();
        if (_patrols.Any(x => x.Id == patrolDto.Id))
            throw new Exception($"Patrol with {patrolDto.Id} is already present.");

        _patrols.Add(_patrolFactory.CreatePatrol(patrolDto));
        _semaphoreSlim.Release();
    }

    public async Task UpdateIncident(UpdateIncidentDto updateIncidentDto)
    {
        await _semaphoreSlim.WaitAsync();
        var incident = _incidents.FirstOrDefault(x => x.Id == updateIncidentDto.Id) ?? throw new Exception($"Incident with ID: {updateIncidentDto.Id} not found");
        incident.Update(updateIncidentDto);
        _semaphoreSlim.Release();
    }

    public async Task UpdatePatrol(PatrolDto patrolDto)
    {
        await _semaphoreSlim.WaitAsync();
        var patrol = _patrols.FirstOrDefault(x => x.Id == patrolDto.Id) ?? throw new Exception($"Patrol with ID: {patrolDto.Id} not found");
        patrol.Update(patrolDto);
        _semaphoreSlim.Release();
    }

    // private async Task NotifyAll()
    // {
    //     foreach (var subscription in _subscriptions)
    //         await subscription.Invoke(this);
    // }
}