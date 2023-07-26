namespace Simulation.Application;

public interface ISimulation
{
    Task RunAsync(CancellationToken? cancellationToken = null);
}