using Shared.Application.Integration.DTOs;

namespace Shared.Application.Integration.Events;

public record IncidentCreatedEvent(NewIncidentDto NewIncidentDto) : IEvent
{
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}