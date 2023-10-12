using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Patrol;

namespace Shared.Domain.DomainEvents.Patrol;

public record PatrolCreated(Guid Id, string PatrolId, Position Position, PatrolStatusEnum Status) : IDomainEvent;