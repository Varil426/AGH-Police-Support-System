using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Patrol;

namespace WebApp.Application.Services.Statistics;

public class PatrolData
{
    public StateHistory<PatrolStatusEnum> History { get; } = new();
    public List<(Position position, DateTimeOffset changedAt)> PositionHistory { get; } = new();
    public string PatrolId { get; init; }

    public PatrolData(string patrolId)
    {
        PatrolId = patrolId;
    }
}