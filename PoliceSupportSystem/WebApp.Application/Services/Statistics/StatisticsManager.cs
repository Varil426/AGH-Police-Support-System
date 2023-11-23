using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Incident;
using Shared.CommonTypes.Patrol;

namespace WebApp.Application.Services.Statistics;

public class StatisticsManager : IStatisticsManager
{
    private List<PatrolData> _patrolData = new();
    private List<IncidentData> _incidentData = new();
    private List<double> _distancesOfConsideredPatrolsFromIncident = new();
    private List<double> _distancesOfChosenPatrolsFromIncident = new();

    private Dictionary<DateTimeOffset, int> _numberOfActiveIncidentsInTime = new();
    private Dictionary<DateTimeOffset, int> _numberOfActiveShootingsInTime = new();

    public void AddIncident(Guid incidentId, Position position, DateTimeOffset createdAt)
    {
        var incidentData = new IncidentData(incidentId, position, createdAt);
        incidentData.History.States.Add((IncidentStatusEnum.AwaitingPatrolArrival, createdAt));
        _incidentData.Add(incidentData);
        UpdateIncidentsInTimeData();
    }

    public void UpdateIncident(Guid incidentId, IncidentStatusEnum statusEnum, DateTimeOffset changedAt)
    {
        _incidentData.First(x => x.IncidentId == incidentId).History.States.Add((statusEnum, changedAt));
        UpdateIncidentsInTimeData();
    }

    public void AddPatrol(string patrolId, Position position)
    {
        var patrolData = new PatrolData(patrolId);
        patrolData.History.States.Add((PatrolStatusEnum.AwaitingOrders, DateTimeOffset.UtcNow));
        patrolData.PositionHistory.Add((position, DateTimeOffset.UtcNow));
        _patrolData.Add(patrolData);
    }

    public void UpdatePatrol(string patrolId, PatrolStatusEnum patrolStatusEnum, DateTimeOffset changedAt) => _patrolData
        .First(x => x.PatrolId.Equals(patrolId, StringComparison.InvariantCultureIgnoreCase)).History.States.Add((patrolStatusEnum, changedAt));

    public void UpdatePatrol(string patrolId, Position position, DateTimeOffset changedAt) => _patrolData
        .First(x => x.PatrolId.Equals(patrolId, StringComparison.InvariantCultureIgnoreCase)).PositionHistory.Add((position, changedAt));

    public void AddDistanceOfConsideredPatrolFromIncident(double distance) => _distancesOfConsideredPatrolsFromIncident.Add(distance);

    public void DistanceOfChosenPatrolFromIncident(double distance) => _distancesOfChosenPatrolsFromIncident.Add(distance);

    private void UpdateIncidentsInTimeData()
    {
        UpdateNumberOfActiveShootingsInTime();
        UpdateNumberOfActiveIncidentsInTime();
    }
    private void UpdateNumberOfActiveShootingsInTime() => _numberOfActiveShootingsInTime[DateTimeOffset.UtcNow] = _incidentData.Count(x => x.History.States.Last().state == IncidentStatusEnum.OnGoingShooting);
    private void UpdateNumberOfActiveIncidentsInTime() => _numberOfActiveShootingsInTime[DateTimeOffset.UtcNow] = _incidentData.Count(x => x.History.States.Last().state is not IncidentStatusEnum.Resolved);
}