using Shared.Domain.Geo;

namespace Shared.Domain.Incident;

public class Incident : BaseRootDomainEntity
{
    public Guid Id { get; }
    public Position Location { get; private set; }
    public IncidentStatusEnum Status { get; private set; }
    public IncidentTypeEnum Type { get; private set; }

    public Incident(Guid id, Position location, IncidentStatusEnum status, IncidentTypeEnum type)
    {
        Location = location;
        Status = status;
        Type = type;
        Id = id;
    }

    public virtual void UpdateLocation(Position newLocation) => Location = newLocation;
    public virtual void UpdateStatus(IncidentStatusEnum newStatus) => Status = newStatus;
    public virtual void UpdateType(IncidentTypeEnum newType) => Type = newType;
}