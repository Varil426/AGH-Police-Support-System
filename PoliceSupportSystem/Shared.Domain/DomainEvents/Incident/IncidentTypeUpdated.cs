using Shared.CommonTypes.Incident;

namespace Shared.Domain.DomainEvents.Incident;

public record IncidentTypeUpdated(Domain.Incident.Incident Incident, IncidentTypeEnum NewType) : IDomainEvent;