using Shared.CommonTypes.Geo;
using Simulation.Application.Entities;

namespace Simulation.Application.Services;

public interface ISimulationPatrolFactory
{
    public SimulationPatrol CreatePatrol(Guid id, string patrolId, Position position);
    public SimulationPatrol CreatePatrol(string patrolId);
    public SimulationPatrol CreatePatrol(string patrolId, Position position) => CreatePatrol(Guid.NewGuid(), patrolId, position);
}