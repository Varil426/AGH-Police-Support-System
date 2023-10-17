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
    // private readonly Stopwatch _lastActionWatch = new();
    // private readonly Stopwatch _simulationStartWatch = new();
    
    private TimeSpan TimeSinceLastAction => DateTimeOffset.UtcNow - _lastActionTime;
    private TimeSpan TimeSinceStart => DateTimeOffset.UtcNow - _simulationStartTime;

    public TimeSpan SimulationTimeSinceStart => /*_simulationStartWatch.Elapsed;*/ TimeSinceStart * _simulationSettings.TimeRate;
    public TimeSpan SimulationTimeSinceLastAction => /*_lastActionWatch.Elapsed;*/ TimeSinceLastAction * _simulationSettings.TimeRate;

    public void Start()
    {
        _simulationStartTime = DateTimeOffset.UtcNow;
        _lastActionTime = _simulationStartTime;
        // _lastActionWatch.Start();
        // _simulationStartWatch.Start();
    }

    public void UpdateLastActionTime() => _lastActionTime = DateTimeOffset.UtcNow;
    // public void UpdateLastActionTime() => _lastActionWatch.Restart();
    
    public TimeSpan TranslateToSimulationTime(DateTimeOffset moment) => (moment - _simulationStartTime) * _simulationSettings.TimeRate;
    // public TimeSpan TranslateToSimulationTime(DateTimeOffset moment)
    // {
    //     TimeSpan result = default;
    //     try
    //     {
    //         result = (moment - _simulationStartTime) * _simulationSettings.TimeRate;
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e.Message);
    //     }
    //
    //     return result;
    // }
    public DateTimeOffset TranslateFromSimulationTime(TimeSpan simulationTime) => _simulationStartTime + simulationTime;
}