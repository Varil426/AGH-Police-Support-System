using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Incident;

namespace WebApp.Application.Services.Statistics;

public class IncidentData
{
    public StateHistory<IncidentStatusEnum> History { get; } = new();
    public DateTimeOffset? ResponseAt => History.States.FirstOrDefault(x => x.state == IncidentStatusEnum.OnGoingNormal).since;
    public DateTimeOffset? ResolvedAt => History.States.FirstOrDefault(x => x.state == IncidentStatusEnum.Resolved).since;
    public bool ChangedIntoFiring => History.States.Any(x => x.state == IncidentStatusEnum.OnGoingShooting);
    public Guid IncidentId { get; init; }
    public Position Position { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    
    public IncidentData(Guid incidentId, Position position, DateTimeOffset createdAt)
    {
        IncidentId = incidentId;
        Position = position;
        CreatedAt = createdAt;
    }
}