namespace Shared.Application.Integration.Events;

public record ChosenPatrolDistanceToIncidentEvent(double Distance) : IEvent
{
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}