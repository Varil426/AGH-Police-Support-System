using Shared.Domain.DomainEvents.Incident;
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

    public virtual void UpdateLocation(Position newLocation)
    {
        if (newLocation == Location)
             return;
        Location = newLocation;
        UpdatedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new IncidentLocationUpdated(Id, newLocation));
    }

    public virtual void UpdateStatus(IncidentStatusEnum newStatus)
    {
        if (newStatus == Status)
            return;
        Status = newStatus;
        UpdatedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new IncidentStatusUpdated(Id, newStatus));
    }

    public virtual void UpdateType(IncidentTypeEnum newType)
    {
        if (newType == Type)
            return;
        Type = newType;
        UpdatedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new IncidentTypeUpdated(Id, newType));
    }
}