using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.CommonTypes.Incident;
using Shared.CommonTypes.Patrol;
using Simulation.Application.Directors.Settings;
using Simulation.Application.Entities;
using Simulation.Application.Helpers;
using Simulation.Application.Services;

namespace Simulation.Application.Directors.IncidentDirector;

internal class IncidentDirector : BackgroundService, IDirector
{
    private readonly ISimulationIncidentFactory _simulationIncidentFactory;
    private readonly IncidentDirectorSettings _incidentDirectorSettings;
    private readonly IMapService _mapService;
    private readonly ILogger<IncidentDirector> _logger;
    private readonly IIncidentRandomizer _randomizer;
    private readonly SimulationSettings _simulationSettings;
    private readonly ISimulationTimeService _simulationTimeService;
    private readonly List<PlannedIncident> _plannedIncidents = new();
    private readonly List<District> _districts = new();
    private readonly TimeSpan _planForSimulationTime = TimeSpan.FromDays(1);

    private readonly SemaphoreSlim _plannedIncidentsSemaphore = new(1, 1);

    private TimeSpan _lastPlannedAt;

    public IncidentDirector(
        ISimulationIncidentFactory simulationIncidentFactory,
        IncidentDirectorSettings incidentDirectorSettings,
        IMapService mapService,
        ILogger<IncidentDirector> logger,
        IIncidentRandomizer randomizer,
        SimulationSettings simulationSettings,
        ISimulationTimeService simulationTimeService)
    {
        _simulationIncidentFactory = simulationIncidentFactory;
        _incidentDirectorSettings = incidentDirectorSettings;
        _mapService = mapService;
        _logger = logger;
        _randomizer = randomizer;
        _simulationSettings = simulationSettings;
        _simulationTimeService = simulationTimeService;
    }

    public async Task Act(ISimulation simulation)
    {
        try
        {
            await _plannedIncidentsSemaphore.WaitAsync();
            StartIncidents(simulation);
            UpdateIncidents(simulation);
            RemoveResolved();
        }
        finally
        {
            _plannedIncidentsSemaphore.Release();
        }
    }

    private async Task PlanIncidents()
    {
        var plannedIncidents = new List<PlannedIncident>();
        var simulationTimeSinceStart = _simulationTimeService.SimulationTimeSinceStart;
        foreach (var district in _districts)
            plannedIncidents.AddRange(await _randomizer.PlanIncidents(district, simulationTimeSinceStart, _planForSimulationTime));

        try
        {
            await _plannedIncidentsSemaphore.WaitAsync();
            _plannedIncidents.AddRange(plannedIncidents);
        }
        finally
        {
            _plannedIncidentsSemaphore.Release();
        }
        
        _lastPlannedAt = simulationTimeSinceStart;
    }

    private void StartIncidents(ISimulation simulation)
    {
        var incidentsThatShouldStart = _plannedIncidents.Where(x => !x.HasStarted && x.ShouldStartAfter <= _simulationTimeService.SimulationTimeSinceStart);
        foreach (var plannedIncident in incidentsThatShouldStart)
        {
            var incident = _simulationIncidentFactory.CreateIncident(
                plannedIncident.IncidentId,
                plannedIncident.IncidentLocation,
                plannedIncident.InitialIncidentType,
                IncidentStatusEnum.WaitingForResponse,
                _simulationTimeService.TranslateFromSimulationTime(plannedIncident.ShouldStartAfter));

            simulation.AddIncident(incident);
            plannedIncident.HasStarted = true;
        }
    }

    private void UpdateIncidents(ISimulation simulation)
    {
        foreach (var incident in simulation.Incidents)
        {
            var plannedIncident = _plannedIncidents.FirstOrDefault(x => x.IncidentId == incident.Id) ?? throw new Exception("Missing planned incident");
            var isInCurrentStateFromSimulationTime = _simulationTimeService.TranslateToSimulationTime(incident.History.Last().UpdatedAt);
            
            switch (incident.Status)
            {
                case IncidentStatusEnum.OnGoingNormal when plannedIncident.ShouldChangeIntoShootingAfter != null:
                    if (isInCurrentStateFromSimulationTime + plannedIncident.ShouldChangeIntoShootingAfter <= _simulationTimeService.SimulationTimeSinceStart)
                    {
                        _logger.LogInformation("Incident {IncidentId} changes into a shooting.", incident.Id);
                        incident.UpdateStatus(IncidentStatusEnum.AwaitingBackup);
                        incident.UpdateType(IncidentTypeEnum.Shooting);
                        
                        incident.RelatedPatrols.ToList().ForEach(x => x.UpdateStatus(PatrolStatusEnum.InShooting));
                    }
                    break;
                case IncidentStatusEnum.OnGoingNormal when plannedIncident.ShouldChangeIntoShootingAfter == null:
                    if (isInCurrentStateFromSimulationTime + plannedIncident.ShouldFinishAfter <= _simulationTimeService.SimulationTimeSinceStart)
                    {
                        _logger.LogInformation("Incident {IncidentId} resolved.", incident.Id);
                        incident.UpdateStatus(IncidentStatusEnum.Resolved);
                        foreach (var patrol in incident.RelatedPatrols.ToList())
                        {
                            patrol.FreePatrol();
                            incident.RemoveRelatedPatrol(patrol);
                        }
                    }
                    break;
                case IncidentStatusEnum.OnGoingShooting:
                    // TODO
                    break;
            }
            
        }
    }

    private void RemoveResolved()
    {
        var resolvedIncidents = _plannedIncidents.Where(x => x.IsResolved).ToList();
        foreach (var resolvedIncident in resolvedIncidents)
            _plannedIncidents.Remove(resolvedIncident);
    }
    
    private async Task PopulateDistricts()
    {
        var districtNames = await _mapService.GetDistrictNames().ToListAsync();
        _districts.AddRange(districtNames.Select(x => new District(x, _incidentDirectorSettings.DistrictDangerLevels.TryGetNullable(x) ?? DistrictDangerLevelEnum.Normal)));
        _logger.LogInformation($"Retrieved district list with {districtNames.Count} element(s)");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_districts.Any())
                await PopulateDistricts();
            await PlanIncidents();

            await Task.Delay(_planForSimulationTime / _simulationSettings.TimeRate, stoppingToken);
        }
    }
}