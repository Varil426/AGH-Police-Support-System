using Shared.Application.Integration.DTOs;

namespace Shared.Application.Integration.Events;

public record IncidentChangedEvent(IncidentDto IncidentDto) : IEvent
{
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}