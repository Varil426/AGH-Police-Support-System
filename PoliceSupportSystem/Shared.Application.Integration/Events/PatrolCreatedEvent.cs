using Shared.Application.Integration.DTOs;

namespace Shared.Application.Integration.Events;

public record PatrolCreatedEvent(NewPatrolDto NewPatrolDto) : IEvent
{
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}