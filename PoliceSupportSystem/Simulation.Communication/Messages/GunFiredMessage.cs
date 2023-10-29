namespace Simulation.Communication.Messages;

public record GunFiredMessage(string Receiver) : IDirectSimulationMessage
{
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}