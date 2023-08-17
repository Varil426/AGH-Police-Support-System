using Simulation.Application.Entities;

namespace Simulation.Application;

public interface ISimulation
{
    IReadOnlyCollection<SimulationIncident> Incidents { get; }
    
    Task RunAsync(CancellationToken? cancellationToken = null);

    void AddService(IService service);

    void RemoveService(string serviceId);

    Task AddIncident(SimulationIncident newIncident);
}