namespace Simulation.Communication.Messages;

public record ServiceOfflineMessage(string ServiceId) : ISimulationMessage
{
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}