namespace Simulation.Communication.Messages;

public record DistrictNameMessage(string? DistrictName) : ISimulationMessage
{
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}