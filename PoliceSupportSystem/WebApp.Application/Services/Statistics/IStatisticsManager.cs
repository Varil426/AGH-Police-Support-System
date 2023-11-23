﻿using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Incident;
using Shared.CommonTypes.Patrol;

namespace WebApp.Application.Services.Statistics;

public interface IStatisticsManager
{
    public void AddIncident(Guid incidentId, Position position, DateTimeOffset createdAt);

    public void UpdateIncident(Guid incidentId, IncidentStatusEnum statusEnum, DateTimeOffset changedAt);
    
    public void AddPatrol(string patrolId, Position position);

    public void UpdatePatrol(string patrolId, PatrolStatusEnum patrolStatusEnum, DateTimeOffset changedAt);
    public void UpdatePatrol(string patrolId, Position position, DateTimeOffset changedAt);
    public void AddDistanceOfConsideredPatrolFromIncident(double distance);
    public void DistanceOfChosenPatrolFromIncident(double distance);
}