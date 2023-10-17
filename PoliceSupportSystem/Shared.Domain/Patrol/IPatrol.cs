using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Patrol;

namespace Shared.Domain.Patrol;

public interface IPatrol : IRootDomainEntity
{
    string PatrolId { get; }
    Guid Id { get; }
    Position Position { get; }
    PatrolStatusEnum Status { get; }
    void UpdateStatus(PatrolStatusEnum newStatus);
    void UpdatePosition(Position newPosition);
}