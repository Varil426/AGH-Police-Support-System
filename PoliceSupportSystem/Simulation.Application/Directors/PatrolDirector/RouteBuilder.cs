using Microsoft.Extensions.Logging;
using Shared.CommonTypes.Geo;
using Simulation.Application.Entities.Patrol;
using Simulation.Application.Services;

namespace Simulation.Application.Directors.PatrolDirector;

internal class RouteBuilder : IRouteBuilder
{
    private readonly IMapService _mapService;
    private readonly ILogger<RouteBuilder> _logger;

    public RouteBuilder(IMapService mapService, ILogger<RouteBuilder> logger)
    {
        _mapService = mapService;
        _logger = logger;
    }

    public async Task<SimulationPatrolRoute> CreateRoute(Position start, Position end)
    {
        var route = await _mapService.GetRoute(start, end);

        if (!route.Steps.Any())
        {
            _logger.LogWarning("{serviceName} returned empty path.", nameof(IMapService));
            return new SimulationPatrolRoute(route.Steps);
        }

        return new SimulationPatrolRoute(route.Steps);
    }
}