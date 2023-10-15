using Simulation.Application.Entities;
using Simulation.Application.Entities.Patrol;

namespace Simulation.Application;

public interface ISimulation
{
    IReadOnlyCollection<SimulationIncident> Incidents { get; }
    IReadOnlyCollection<SimulationPatrol> Patrols { get; }
    
    Task RunAsync(CancellationToken? cancellationToken = null);

    void AddService(IService service);

    void AddPatrol(SimulationPatrol patrol);

    void RemoveService(string serviceId);

    void RemovePatrol(string patrolId);

    void AddIncident(SimulationIncident newIncident);
}