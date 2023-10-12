namespace Simulation.Communication.Messages;

public record PatrolDistrictMessage(string DistrictName, string PatrolId) : ISimulationMessage
{
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}