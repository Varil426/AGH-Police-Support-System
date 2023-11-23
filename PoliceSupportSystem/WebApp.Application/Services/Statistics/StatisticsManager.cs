using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Incident;
using Shared.CommonTypes.Patrol;

namespace WebApp.Application.Services.Statistics;

public class StatisticsManager : IStatisticsManager
{
    private List<PatrolData> _patrolData = new();
    private List<IncidentData> _incidentData = new();

    public void AddIncident(Guid incidentId, Position position, DateTimeOffset createdAt)
    {
        var incidentData = new IncidentData(incidentId, position, createdAt);
        incidentData.History.States.Add((IncidentStatusEnum.AwaitingPatrolArrival, createdAt));
        _incidentData.Add(incidentData);
    }

    public void UpdateIncident(Guid incidentId, IncidentStatusEnum statusEnum, DateTimeOffset changedAt) =>
        _incidentData.First(x => x.IncidentId == incidentId).History.States.Add((statusEnum, changedAt));

    public void AddPatrol(string patrolId)
    {
        var patrolData = new PatrolData(patrolId);
        patrolData.History.States.Add((PatrolStatusEnum.AwaitingOrders, DateTimeOffset.UtcNow));
        _patrolData.Add(patrolData);
    }

    public void UpdatePatrol(string patrolId, PatrolStatusEnum patrolStatusEnum, DateTimeOffset changedAt) => _patrolData
        .First(x => x.PatrolId.Equals(patrolId, StringComparison.InvariantCultureIgnoreCase)).History.States.Add((patrolStatusEnum, changedAt));
}