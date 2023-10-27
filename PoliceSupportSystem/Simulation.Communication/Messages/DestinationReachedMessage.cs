namespace Simulation.Communication.Messages;

public record DestinationReachedMessage(string Receiver) : IDirectSimulationMessage
{
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}