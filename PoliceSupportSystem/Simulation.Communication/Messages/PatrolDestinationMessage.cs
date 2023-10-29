using Shared.CommonTypes.Geo;

namespace Simulation.Communication.Messages;

public record PatrolDestinationMessage(Position Position, string PatrolId, bool IsEmergency) : ISimulationMessage
{
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}