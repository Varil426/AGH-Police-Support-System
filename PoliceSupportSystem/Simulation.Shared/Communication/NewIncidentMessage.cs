using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Incident;

namespace Simulation.Shared.Communication;

public record NewIncidentMessage(Guid Id, Position Location, IncidentTypeEnum Type = IncidentTypeEnum.NormalIncident, IncidentStatusEnum Status = IncidentStatusEnum.WaitingForResponse) : ISimulationMessage
{
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}