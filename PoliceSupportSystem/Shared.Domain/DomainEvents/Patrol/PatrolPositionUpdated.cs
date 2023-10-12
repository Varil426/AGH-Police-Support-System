using Shared.CommonTypes.Geo;

namespace Shared.Domain.DomainEvents.Patrol;

public record PatrolPositionUpdated(Domain.Patrol.Patrol Patrol, Position NewPosition) : IDomainEvent;