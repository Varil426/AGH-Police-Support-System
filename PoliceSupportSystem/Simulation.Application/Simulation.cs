using Microsoft.Extensions.Logging;
using Simulation.Application.Directors;
using Simulation.Application.Entities;
using Simulation.Application.Services;

namespace Simulation.Application;

internal class Simulation : ISimulation
{
    private readonly ILogger<Simulation> _logger;
    private readonly SimulationSettings _simulationSettings;
    private readonly IMessageService _messageService;
    private readonly ISimulationMessageProcessor _simulationMessageProcessor;
    private readonly IReadOnlyCollection<IDirector> _directors;

    private readonly List<IService> _services = new();
    private DateTimeOffset _lastActionTime;
    private DateTimeOffset _simulationStartTime;

    private TimeSpan TimeSinceLastAction => DateTimeOffset.Now - _lastActionTime;
    private TimeSpan TimeSinceStart => DateTimeOffset.Now - _simulationStartTime;
    public TimeSpan SimulationTimeSinceStart => TimeSinceStart * _simulationSettings.TimeRate;
    public TimeSpan SimulationTimeSinceLastAction => TimeSinceLastAction * _simulationSettings.TimeRate;

    public Simulation(ILogger<Simulation> logger, SimulationSettings simulationSettings, IMessageService messageService, ISimulationMessageProcessor simulationMessageProcessor, IReadOnlyCollection<IDirector> directors)
    {
        _logger = logger;
        _simulationSettings = simulationSettings;
        _messageService = messageService;
        _simulationMessageProcessor = simulationMessageProcessor;
        _directors = directors;
    }

    public async Task RunAsync(CancellationToken? cancellationToken = null)
    {
        _simulationStartTime = DateTimeOffset.Now;
        _lastActionTime = _simulationStartTime;
        _logger.LogInformation("Simulation has started");
        while (cancellationToken is { IsCancellationRequested: false } or null)
        {
            // Receive messages
            var messages = (await _messageService.GetMessagesAsync()).OrderBy(x => x.CreatedAt);
            // Process messages - Update state
            await _simulationMessageProcessor.ProcessAsync(this, messages);
            // TODO Perform actions
            _lastActionTime = DateTimeOffset.Now;
            await Task.WhenAll(_directors.Select(x => x.InvokeAsync(this)));
            // TODO Send updates
            await Task.Delay(30);
        }
    }

    public void AddService(IService service)
    {
        if (!_services.Select(x => x.Id).Any(x => x.Equals(service.Id, StringComparison.InvariantCultureIgnoreCase)))
            _services.Add(service);
        else
            _logger.LogWarning($"Attempted to add a duplicated service with ID: {service.Id}");
    }

    public void RemoveService(string serviceId)
    {
        var s = _services.FirstOrDefault(x => x.Id.Equals(serviceId, StringComparison.InvariantCultureIgnoreCase));
        if (s is not null)
            _services.Remove(s);
        else
            _logger.LogWarning($"Attempted to remove not existing service with ID: {serviceId}");
    }

    public TimeSpan TranslateToSimulationTime(DateTimeOffset moment) => (moment - _simulationStartTime) * _simulationSettings.TimeRate;
}