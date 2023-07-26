using Simulation.Application;

namespace Simulation.Simulation;

internal class SimulationRunner : BackgroundService
{
    private readonly ISimulation _simulation;


    public SimulationRunner(ISimulation simulation)
    {
        _simulation = simulation;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) => _simulation.RunAsync();
}