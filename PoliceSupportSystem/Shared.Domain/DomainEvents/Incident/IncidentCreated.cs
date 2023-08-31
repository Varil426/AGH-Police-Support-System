using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Incident;

namespace Shared.Domain.DomainEvents.Incident;

public record IncidentCreated(Guid IncidentId, Position Location, IncidentStatusEnum Status, IncidentTypeEnum Type) : IDomainEvent;