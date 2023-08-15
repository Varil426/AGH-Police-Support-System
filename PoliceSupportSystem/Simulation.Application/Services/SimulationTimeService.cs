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
    
    private TimeSpan TimeSinceLastAction => DateTimeOffset.Now - _lastActionTime;
    private TimeSpan TimeSinceStart => DateTimeOffset.Now - _simulationStartTime;
    
    public TimeSpan SimulationTimeSinceStart => TimeSinceStart * _simulationSettings.TimeRate;
    public TimeSpan SimulationTimeSinceLastAction => TimeSinceLastAction * _simulationSettings.TimeRate;

    public void Start()
    {
        _simulationStartTime = DateTimeOffset.Now;
        _lastActionTime = _simulationStartTime;
    }

    public void UpdateLastActionTime() => _lastActionTime = DateTimeOffset.Now;
    
    public TimeSpan TranslateToSimulationTime(DateTimeOffset moment) => (moment - _simulationStartTime) * _simulationSettings.TimeRate;
}