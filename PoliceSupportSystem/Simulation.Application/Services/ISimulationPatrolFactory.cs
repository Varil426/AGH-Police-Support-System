using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Patrol;
using Simulation.Application.Entities;

namespace Simulation.Application.Services;

public interface ISimulationPatrolFactory
{
    public SimulationPatrol CreatePatrol(Guid id, string patrolId, Position position, PatrolStatusEnum status);
    public SimulationPatrol CreatePatrol(string patrolId);
    public SimulationPatrol CreatePatrol(string patrolId, Position position, PatrolStatusEnum status) => CreatePatrol(Guid.NewGuid(), patrolId, position, status);
}