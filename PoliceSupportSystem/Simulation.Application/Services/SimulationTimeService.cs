namespace Simulation.Application.Services;

internal class SimulationTimeService : ISimulationTimeService
{
    private readonly SimulationSettings _simulationSettings;

    public SimulationTimeService(SimulationSettings simulationSettings)
    {
        _simulationSettings = simulationSettings;
    }

    private DateTimeOffset _lastActionTime;
    private DateTimeOffset _simulationStartTime;

    private TimeSpan TimeSinceLastAction => DateTimeOffset.UtcNow - _lastActionTime;
    private TimeSpan TimeSinceStart => DateTimeOffset.UtcNow - _simulationStartTime;

    public TimeSpan SimulationTimeSinceStart => TimeSinceStart * _simulationSettings.TimeRate;
    public TimeSpan SimulationTimeSinceLastAction => TimeSinceLastAction * _simulationSettings.TimeRate;
    public double SimulationTimeRate => _simulationSettings.TimeRate;

    public void Start()
    {
        _simulationStartTime = DateTimeOffset.UtcNow;
        _lastActionTime = _simulationStartTime;
    }

    public void UpdateLastActionTime() => _lastActionTime = DateTimeOffset.UtcNow;

    public TimeSpan TranslateToSimulationTime(DateTimeOffset moment) => (moment - _simulationStartTime) * _simulationSettings.TimeRate;
    public DateTimeOffset TranslateFromSimulationTime(TimeSpan simulationTime) => _simulationStartTime + simulationTime;
}