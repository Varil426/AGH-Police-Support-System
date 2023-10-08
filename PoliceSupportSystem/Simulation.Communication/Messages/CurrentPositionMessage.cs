using Shared.CommonTypes.Geo;

namespace Simulation.Communication.Messages;

public record CurrentPositionMessage(string Receiver, Position Position) : IDirectSimulationMessage
{
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}