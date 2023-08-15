using Simulation.Application.Entities;

namespace Simulation.Application;

public interface ISimulation
{
    Task RunAsync(CancellationToken? cancellationToken = null);

    void AddService(IService service);

    void RemoveService(string serviceId);
}