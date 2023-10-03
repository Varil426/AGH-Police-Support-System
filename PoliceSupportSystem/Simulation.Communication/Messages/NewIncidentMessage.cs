using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Incident;

namespace Simulation.Communication.Messages;

public record NewIncidentMessage(Guid Id, Position Location, IncidentTypeEnum Type = IncidentTypeEnum.NormalIncident, IncidentStatusEnum Status = IncidentStatusEnum.WaitingForResponse) : ISimulationMessage
{
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}