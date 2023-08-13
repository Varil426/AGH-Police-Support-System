using Microsoft.Extensions.Logging;
using Simulation.Application.Directors.Settings;
using Simulation.Application.Entities;
using Simulation.Application.Helpers;
using Simulation.Application.Services;

namespace Simulation.Application.Directors.IncidentDirector;

public class IncidentDirector : IDirector
{
    private readonly IEntityFactory _entityFactory;
    private readonly IncidentDirectorSettings _incidentDirectorSettings;
    private readonly IMapService _mapService;
    private readonly ILogger<IncidentDirector> _logger;

    private readonly List<District> _districts = new();

    public IncidentDirector(IEntityFactory entityFactory, IncidentDirectorSettings incidentDirectorSettings, IMapService mapService, ILogger<IncidentDirector> logger)
    {
        _entityFactory = entityFactory;
        _incidentDirectorSettings = incidentDirectorSettings;
        _mapService = mapService;
        _logger = logger;
    }

    public async Task InvokeAsync(ISimulation simulation)
    {
        if (!_districts.Any())
            await PopulateDistricts();
        PlanIncidents(simulation);
        CreateIncidents(simulation);
        UpdateIncidents(simulation);
    }
    
    private void PlanIncidents(ISimulation simulation)
    {
        // TODO
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
        _districts.AddRange(districtNames.Select(x => new District(x, _incidentDirectorSettings.DistrictDangerLevels.TryGet(x))));
        _logger.LogInformation($"Retrieved district list with {districtNames.Count} element(s)");
    }

}