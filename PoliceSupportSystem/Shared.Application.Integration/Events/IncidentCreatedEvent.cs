using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Incident;

namespace Shared.Application.Integration.Events;

public record IncidentCreatedEvent(Guid IncidentId, Position Location, IncidentStatusEnum Status, IncidentTypeEnum Type) : IEvent
{
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}