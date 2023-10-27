using Shared.CommonTypes.Incident;

namespace Simulation.Communication.Messages;

public record IncidentStatusUpdatedMessage(Guid Id, Guid IncidentId, IncidentStatusEnum NewStatus, DateTimeOffset CreatedAt) : ISimulationMessage;