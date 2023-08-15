using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Simulation.Application.Directors.Settings;
using Simulation.Application.Entities;
using Simulation.Application.Helpers;
using Simulation.Application.Services;

namespace Simulation.Application.Directors.IncidentDirector;

internal class IncidentDirector : BackgroundService, IDirector
{
    private readonly IEntityFactory _entityFactory;
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
        IEntityFactory entityFactory,
        IncidentDirectorSettings incidentDirectorSettings,
        IMapService mapService,
        ILogger<IncidentDirector> logger,
        IIncidentRandomizer randomizer,
        SimulationSettings simulationSettings,
        ISimulationTimeService simulationTimeService)
    {
        _entityFactory = entityFactory;
        _incidentDirectorSettings = incidentDirectorSettings;
        _mapService = mapService;
        _logger = logger;
        _randomizer = randomizer;
        _simulationSettings = simulationSettings;
        _simulationTimeService = simulationTimeService;
    }

    public async Task Act(ISimulation simulation)
    {
        await _plannedIncidentsSemaphore.WaitAsync();
        CreateIncidents(simulation);
        UpdateIncidents(simulation);
        _plannedIncidentsSemaphore.Release();
    }

    private async Task PlanIncidents()
    {
        var plannedIncidents = new List<PlannedIncident>();
        var simulationTimeSinceStart = _simulationTimeService.SimulationTimeSinceStart;
        foreach (var district in _districts)
            plannedIncidents.AddRange(await _randomizer.PlanIncidents(district, simulationTimeSinceStart, _planForSimulationTime));

        await _plannedIncidentsSemaphore.WaitAsync();
        _plannedIncidents.AddRange(plannedIncidents);
        _plannedIncidentsSemaphore.Release();
        
        _lastPlannedAt = simulationTimeSinceStart;
    }

    private void CreateIncidents(ISimulation simulation)
    {
        // TODO
    }

    private void UpdateIncidents(ISimulation simulation)
    {
        // TODO
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