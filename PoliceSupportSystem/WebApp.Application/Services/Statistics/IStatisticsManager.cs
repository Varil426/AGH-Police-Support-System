using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Incident;
using Shared.CommonTypes.Patrol;

namespace WebApp.Application.Services.Statistics;

public interface IStatisticsManager
{
    IReadOnlyDictionary<DateTimeOffset, (int numberOfIncidents, int numberOfShootings)> IncidentsInTime { get; }
    IReadOnlyCollection<PatrolData> PatrolData { get; }
    IReadOnlyCollection<IncidentData> IncidentData { get; }
    IReadOnlyCollection<double> DistancesOfConsideredPatrolsFromIncident { get; }
    IReadOnlyCollection<double> DistancesOfChosenPatrolsFromIncident { get; }
    
    public void AddIncident(Guid incidentId, Position position, DateTimeOffset createdAt);

    public void UpdateIncident(Guid incidentId, IncidentStatusEnum statusEnum, DateTimeOffset changedAt);
    
    public void AddPatrol(string patrolId, Position position);

    public void UpdatePatrol(string patrolId, PatrolStatusEnum patrolStatusEnum, DateTimeOffset changedAt);
    public void UpdatePatrol(string patrolId, Position position, DateTimeOffset changedAt);
    public void AddDistanceOfConsideredPatrolFromIncident(double distance);
    public void AddDistanceOfChosenPatrolFromIncident(double distance);
}