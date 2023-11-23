using Shared.CommonTypes.District;
using Simulation.Application.Entities;

namespace Simulation.Application.Directors.IncidentDirector;

internal interface IIncidentRandomizer
{
    int DetermineNumberOfIncidentsForDay(DistrictDangerLevelEnum dangerLevel);

    bool ShouldChangeIntoShooting(DistrictDangerLevelEnum dangerLevel);

    Task<IEnumerable<PlannedIncident>> PlanIncidents(District district, TimeSpan currentSimulationTime, TimeSpan planAheadFor);
}