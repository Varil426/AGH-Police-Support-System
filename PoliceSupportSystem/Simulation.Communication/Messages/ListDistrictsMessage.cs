namespace Simulation.Communication.Messages;

public record ListDistrictsMessage(IEnumerable<string> Districts) : ISimulationMessage
{
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}