using Shared.CommonTypes.Patrol;

namespace Shared.Domain.DomainEvents.Patrol;

public record PatrolStatusUpdated(Domain.Patrol.Patrol Patrol, PatrolStatusEnum NewStatus) : IDomainEvent;