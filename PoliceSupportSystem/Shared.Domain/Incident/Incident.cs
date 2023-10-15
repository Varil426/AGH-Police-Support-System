using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Incident;
using Shared.Domain.DomainEvents.Incident;

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
        AddDomainEvent(new IncidentCreated(Id, Location, Status, Type));
    }

    public virtual void UpdateLocation(Position newLocation)
    {
        if (newLocation == Location)
             return;
        Location = newLocation;
        UpdateUpdatedAt();
        AddDomainEvent(new IncidentLocationUpdated(Id, newLocation));
    }

    public virtual void UpdateStatus(IncidentStatusEnum newStatus)
    {
        if (newStatus == Status)
            return;
        Status = newStatus;
        UpdateUpdatedAt();
        AddDomainEvent(new IncidentStatusUpdated(Id, newStatus));
    }

    public virtual void UpdateType(IncidentTypeEnum newType)
    {
        if (newType == Type)
            return;
        Type = newType;
        UpdateUpdatedAt();
        AddDomainEvent(new IncidentTypeUpdated(Id, newType));
    }
}