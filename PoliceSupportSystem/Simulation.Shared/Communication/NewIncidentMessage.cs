using Shared.Domain.Geo;
using Shared.Domain.Incident;

namespace Simulation.Shared.Communication;

public record NewIncidentMessage(Guid Id, Position Location, IncidentTypeEnum Type = IncidentTypeEnum.NormalIncident, IncidentStatusEnum Status = IncidentStatusEnum.WaitingForResponse) : ISimulationMessage
{
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.Now;
}