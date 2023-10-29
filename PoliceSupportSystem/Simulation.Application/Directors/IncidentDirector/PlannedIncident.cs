using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Incident;

namespace Simulation.Application.Directors.IncidentDirector;

internal record PlannedIncident(
    Guid IncidentId,
    Position IncidentLocation,
    TimeSpan ShouldStartAfter,
    IncidentTypeEnum InitialIncidentType,
    TimeSpan ShouldFinishAfter,
    TimeSpan? ShouldChangeIntoShootingAfter,
    int? NumberOfPatrolsRequiredToSolve = null)
{
    public bool HasStarted { get; set; } = default;
    public bool HasShootingStarted { get; set; } = default;
    public bool IsResolved { get; set; } = default;
}