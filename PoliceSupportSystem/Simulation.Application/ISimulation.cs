using Simulation.Application.Entities;

namespace Simulation.Application;

public interface ISimulation
{
    TimeSpan SimulationTimeSinceStart { get; }
    TimeSpan SimulationTimeSinceLastAction { get; }
    
    Task RunAsync(CancellationToken? cancellationToken = null);

    void AddService(IService service);

    void RemoveService(string serviceId);

    TimeSpan TranslateToSimulationTime(DateTimeOffset moment);
}