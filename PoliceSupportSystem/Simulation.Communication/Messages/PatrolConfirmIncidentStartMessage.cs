namespace Simulation.Communication.Messages;

public record PatrolConfirmIncidentStartMessage(string PatrolId, Guid IncidentId) : ISimulationMessage
{
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}