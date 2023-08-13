using Shared.Domain.Geo;
using Shared.Domain.Incident;

namespace Simulation.Application.Directors.IncidentDirector;

internal record PlannedIncident(
    Guid IncidentId,
    Position IncidentLocation,
    TimeSpan ShouldStartAfter,
    IncidentTypeEnum InitialIncidentType,
    TimeSpan ShouldFinishAfter,
    TimeSpan? ShouldChangeIntoShootingAfter);