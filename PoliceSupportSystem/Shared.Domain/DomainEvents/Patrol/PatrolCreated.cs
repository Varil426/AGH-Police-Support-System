using Shared.CommonTypes.Geo;

namespace Shared.Domain.DomainEvents.Patrol;

public record PatrolCreated(Guid Id, string PatrolId, Position Position) : IDomainEvent;