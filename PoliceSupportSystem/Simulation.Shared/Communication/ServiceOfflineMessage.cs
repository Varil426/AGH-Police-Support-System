namespace Simulation.Shared.Communication;

public record ServiceOfflineMessage(string ServiceId) : ISimulationMessage
{
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}