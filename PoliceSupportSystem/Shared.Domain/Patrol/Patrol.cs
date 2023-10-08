using Shared.CommonTypes.Geo;
using Shared.Domain.DomainEvents.Patrol;

namespace Shared.Domain.Patrol;

public class Patrol : BaseRootDomainEntity
{
    public string PatrolId { get; }
    
    public Guid Id { get; }
    
    public Position Position { get; private set; }

    public Patrol( Guid id, string patrolId, Position position)
    {
        PatrolId = patrolId;
        Position = position;
        Id = id;
        AddDomainEvent(new PatrolCreated(Id, PatrolId, Position));
    }
}