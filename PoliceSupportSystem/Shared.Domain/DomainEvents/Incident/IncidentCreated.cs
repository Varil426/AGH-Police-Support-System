using Shared.Domain.Geo;
using Shared.Domain.Incident;

namespace Shared.Domain.DomainEvents.Incident;

public record IncidentCreated(Guid IncidentId, Position Location, IncidentStatusEnum Status, IncidentTypeEnum Type) : IDomainEvent;