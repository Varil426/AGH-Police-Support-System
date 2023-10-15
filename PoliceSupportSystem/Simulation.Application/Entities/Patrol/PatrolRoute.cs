using Shared.CommonTypes.Geo;
using Path = Shared.CommonTypes.Geo.Path;

namespace Simulation.Application.Entities.Patrol;

public record SimulationPatrolRoute(Position Start, Position End, IList<Path> Steps) : Route(Steps)
{
    public int LastStepIndex { get; set; } = 0;

    public bool DestinationReached => LastStepIndex >= Steps.Count;

    public Path CurrentStep => Steps[LastStepIndex];
}