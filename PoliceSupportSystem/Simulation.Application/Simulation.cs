namespace Simulation.Application;

public class Simulation : ISimulation
{
    public async Task RunAsync(CancellationToken? cancellationToken = null)
    {
        while (cancellationToken is { IsCancellationRequested: false } or null)
        {
            
        }
    }
}