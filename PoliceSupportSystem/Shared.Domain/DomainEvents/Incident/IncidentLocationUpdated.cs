using Shared.Domain.Geo;

namespace Shared.Domain.DomainEvents.Incident;

public record IncidentLocationUpdated(Guid IncidentId, Position NewLocation) : IDomainEvent;