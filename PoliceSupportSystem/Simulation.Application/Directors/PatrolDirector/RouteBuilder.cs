using Microsoft.Extensions.Logging;
using Shared.CommonTypes.Geo;
using Shared.Domain.Helpers;
using Simulation.Application.Entities.Patrol;
using Simulation.Application.Services;
using Path = Shared.CommonTypes.Geo.Path;

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
            return new SimulationPatrolRoute(start, end, route.Steps);
        }
        
        var firstNode = route.Steps.First();
        route.Steps.Insert(0, new Path(start, firstNode.From, start.GetDistanceTo(firstNode.From)));

        var lastNode = route.Steps.Last();
        route.Steps.Add(new Path(lastNode.To, end, lastNode.To.GetDistanceTo(end)));

        return new SimulationPatrolRoute(start, end, route.Steps);
    }
}