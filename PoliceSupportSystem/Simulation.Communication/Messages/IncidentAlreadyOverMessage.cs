namespace Simulation.Communication.Messages;

public record IncidentAlreadyOverMessage(Guid IncidentId, string Receiver) : IDirectSimulationMessage
{
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}