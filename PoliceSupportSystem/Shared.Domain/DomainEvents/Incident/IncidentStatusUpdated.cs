using Shared.Domain.Incident;

namespace Shared.Domain.DomainEvents.Incident;

public record IncidentStatusUpdated(Guid IncidentId, IncidentStatusEnum NewStatus) : IDomainEvent;