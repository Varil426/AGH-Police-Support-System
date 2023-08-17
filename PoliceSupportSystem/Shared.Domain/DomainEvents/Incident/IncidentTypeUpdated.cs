using Shared.Domain.Incident;

namespace Shared.Domain.DomainEvents.Incident;

public record IncidentTypeUpdated(Guid IncidentId, IncidentTypeEnum NewType) : IDomainEvent;