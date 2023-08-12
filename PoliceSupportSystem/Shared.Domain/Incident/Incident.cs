using Shared.Domain.Geo;

namespace Shared.Domain.Incident;

public class Incident
{
    public Guid Id { get; }
    public Position Location { get; /*private*/ set; }
    public IncidentStatusEnum Status { get; /*private*/ set; }
    public IncidentTypeEnum Type { get; /*private*/ set; }

    public Incident(Guid id, Position location, IncidentStatusEnum status, IncidentTypeEnum type)
    {
        Location = location;
        Status = status;
        Type = type;
        Id = id;
    }
}