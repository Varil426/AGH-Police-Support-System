using Microsoft.Extensions.Logging;
using Simulation.Application.Directors.Settings;
using Simulation.Application.Entities;
using Simulation.Application.Helpers;
using Simulation.Application.Services;

namespace Simulation.Application.Directors.IncidentDirector;

internal class IncidentDirector : IDirector
{
    private readonly IEntityFactory _entityFactory;
    private readonly IncidentDirectorSettings _incidentDirectorSettings;
    private readonly IMapService _mapService;
    private readonly ILogger<IncidentDirector> _logger;
    private readonly IIncidentRandomizer _randomizer;
    private readonly List<PlannedIncident> _plannedIncidents = new();
    private readonly List<District> _districts = new();
    private readonly TimeSpan _planFor = TimeSpan.FromDays(1);

    private TimeSpan _lastPlannedAt;

    public IncidentDirector(
        IEntityFactory entityFactory,
        IncidentDirectorSettings incidentDirectorSettings,
        IMapService mapService,
        ILogger<IncidentDirector> logger,
        IIncidentRandomizer randomizer)
    {
        _entityFactory = entityFactory;
        _incidentDirectorSettings = incidentDirectorSettings;
        _mapService = mapService;
        _logger = logger;
        _randomizer = randomizer;
    }

    public async Task InvokeAsync(ISimulation simulation)
    {
        if (!_districts.Any())
            await PopulateDistricts();
        if (_lastPlannedAt + _planFor < simulation.SimulationTimeSinceStart || _lastPlannedAt == default)
            await PlanIncidents(simulation);
        CreateIncidents(simulation);
        UpdateIncidents(simulation);
    }

    private async Task PlanIncidents(ISimulation simulation)
    {
        // TODO Optimize?
        var simulationTimeSinceStart = simulation.SimulationTimeSinceStart;
        foreach (var district in _districts)
            _plannedIncidents.AddRange(await _randomizer.PlanIncidents(district, simulationTimeSinceStart, _planFor));

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
}