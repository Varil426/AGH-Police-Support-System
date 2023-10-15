using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Patrol;
using Shared.Domain.DomainEvents.Patrol;

namespace Shared.Domain.Patrol;

public class Patrol : BaseRootDomainEntity
{
    public string PatrolId { get; }

    public Guid Id { get; }

    public Position Position { get; private set; }

    public PatrolStatusEnum Status { get; private set; }

    public Patrol(Guid id, string patrolId, Position position, PatrolStatusEnum status)
    {
        PatrolId = patrolId;
        Position = position;
        Status = status;
        Id = id;
        AddDomainEvent(new PatrolCreated(Id, PatrolId, Position, Status));
    }

    public void UpdateStatus(PatrolStatusEnum newStatus)
    {
        if (newStatus == Status)
            return;
        UpdateUpdatedAt();
        Status = newStatus;
        AddDomainEvent(new PatrolStatusUpdated(this, newStatus));
    }

    public void UpdatePosition(Position newPosition)
    {
        if (newPosition == Position)
            return;
        UpdateUpdatedAt();
        Position = newPosition;
        AddDomainEvent(new PatrolPositionUpdated(this, newPosition));
    }
}