using Newtonsoft.Json;
using Shared.CommonTypes.Geo;

namespace Simulation.Communication.Messages;

public record PatrolPositionChangedMessage(Position NewPosition, string Receiver) : IDirectSimulationMessage
{
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;

    // TODO Should be in every message
    [JsonConstructor]
    public PatrolPositionChangedMessage(Position newPosition, string receiver, DateTimeOffset createdAt) : this(newPosition, receiver)
    {
        CreatedAt = createdAt;
    }
}