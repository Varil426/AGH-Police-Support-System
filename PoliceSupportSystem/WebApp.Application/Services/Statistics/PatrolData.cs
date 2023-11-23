using Shared.CommonTypes.Patrol;

namespace WebApp.Application.Services.Statistics;

public class PatrolData
{
    public StateHistory<PatrolStatusEnum> History = new();
    public string PatrolId { get; init; }

    public PatrolData(string patrolId)
    {
        PatrolId = patrolId;
    }
}