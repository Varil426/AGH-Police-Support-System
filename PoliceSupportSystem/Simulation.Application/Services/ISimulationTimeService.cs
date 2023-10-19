namespace Simulation.Application.Services;

public interface ISimulationTimeService
{
    public TimeSpan SimulationTimeSinceStart { get; }
    public TimeSpan SimulationTimeSinceLastAction { get; }
    double SimulationTimeRate { get; }
    void Start();
    void UpdateLastActionTime();
    TimeSpan TranslateToSimulationTime(DateTimeOffset moment);
    DateTimeOffset TranslateFromSimulationTime(TimeSpan simulationTime);
}