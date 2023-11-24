namespace Shared.Application.Integration.Events;

public record PatrolsDistanceToIncidentEvent(IEnumerable<double> Distances) : IEvent
{
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}