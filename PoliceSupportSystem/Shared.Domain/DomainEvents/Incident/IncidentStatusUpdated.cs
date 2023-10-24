using Shared.CommonTypes.Incident;

namespace Shared.Domain.DomainEvents.Incident;

public record IncidentStatusUpdated(Domain.Incident.Incident Incident, IncidentStatusEnum NewStatus) : IDomainEvent;