using Shared.CommonTypes.Geo;
using Simulation.Application.Entities.Patrol;

namespace Simulation.Application.Directors.PatrolDirector;

public interface IRouteBuilder
{
    public Task<SimulationPatrolRoute> CreateRoute(Position start, Position end);
}