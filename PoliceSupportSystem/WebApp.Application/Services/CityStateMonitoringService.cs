using Shared.Application.Factories;
using Shared.Application.Helpers;
using Shared.Application.Integration.DTOs;
using Shared.Application.Services;
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
    private readonly IDomainEventProcessor _domainEventProcessor;

    private readonly List<Incident> _incidents = new();
    private readonly List<Patrol> _patrols = new();
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    public Position HqLocation => _mapSettings.HqLocation;
    public IReadOnlyCollection<Incident> Incidents
    {
        get
        {
            _semaphoreSlim.Wait();
            var result = _incidents.ToList().AsReadOnly();
            _semaphoreSlim.Release();
            return result;
        }
    }

    public IReadOnlyCollection<Patrol> Patrols
    {
        get
        {
            _semaphoreSlim.Wait();
            var result = _patrols.ToList().AsReadOnly();
            _semaphoreSlim.Release();
            return result;
        }
    }

    public CityStateMonitoringService(IIncidentFactory incidentFactory, MapSettings mapSettings, IPatrolFactory patrolFactory, IDomainEventProcessor domainEventProcessor)
    {
        _incidentFactory = incidentFactory;
        _mapSettings = mapSettings;
        _patrolFactory = patrolFactory;
        _domainEventProcessor = domainEventProcessor;
    }


    public async Task AddIncident(NewIncidentDto newIncidentDto)
    {
        await _semaphoreSlim.WaitAsync();
        if (_incidents.Any(x => x.Id == newIncidentDto.Id))
            throw new Exception($"Incident with {newIncidentDto.Id} is already present.");

        var newIncident = _incidentFactory.CreateIncident(newIncidentDto);
        _incidents.Add(newIncident);
        await _domainEventProcessor.ProcessDomainEvents(newIncident);

        _semaphoreSlim.Release();
    }

    public async Task AddPatrol(NewPatrolDto patrolDto)
    {
        await _semaphoreSlim.WaitAsync();
        if (_patrols.Any(x => x.Id == patrolDto.Id))
            throw new Exception($"Patrol with {patrolDto.Id} is already present.");

        var newPatrol = _patrolFactory.CreatePatrol(patrolDto);
        _patrols.Add(newPatrol);
        await _domainEventProcessor.ProcessDomainEvents(newPatrol);

        _semaphoreSlim.Release();
    }

    public async Task UpdateIncident(IncidentDto incidentDto)
    {
        await _semaphoreSlim.WaitAsync();
        var incident = _incidents.FirstOrDefault(x => x.Id == incidentDto.Id) ?? throw new Exception($"Incident with ID: {incidentDto.Id} not found");

        incident.Update(incidentDto);

        await _domainEventProcessor.ProcessDomainEvents(incident);

        _semaphoreSlim.Release();
    }

    public async Task UpdatePatrol(PatrolDto patrolDto)
    {
        await _semaphoreSlim.WaitAsync();
        var patrol = _patrols.FirstOrDefault(x => x.Id == patrolDto.Id) ?? throw new Exception($"Patrol with ID: {patrolDto.Id} not found");
        patrol.Update(patrolDto);
        await _domainEventProcessor.ProcessDomainEvents(patrol);
        _semaphoreSlim.Release();
    }
}