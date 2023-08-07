using Microsoft.Extensions.Logging;
using Simulation.Application.Entities;
using Simulation.Application.Services;
using Simulation.Shared.Common;
using Simulation.Shared.Communication;

namespace Simulation.Application;

public class Simulation : ISimulation
{
    private readonly ILogger<Simulation> _logger;
    private readonly SimulationSettings _simulationSettings;
    private readonly IMessageService _messageService;
    private readonly ISimulationMessageProcessor _simulationMessageProcessor;

    private readonly List<IService> _services = new();
    private DateTimeOffset _lastActionTime;
    private DateTimeOffset _simulationStartTime;

    private TimeSpan TimeSinceLastAction => DateTimeOffset.Now - _lastActionTime;

    private TimeSpan SimulationTimeSinceLastAction => TimeSinceLastAction * _simulationSettings.TimeRate;

    public Simulation(ILogger<Simulation> logger, SimulationSettings simulationSettings, IMessageService messageService, ISimulationMessageProcessor simulationMessageProcessor)
    {
        _logger = logger;
        _simulationSettings = simulationSettings;
        _messageService = messageService;
        _simulationMessageProcessor = simulationMessageProcessor;
    }

    public async Task RunAsync(CancellationToken? cancellationToken = null)
    {
        _simulationStartTime = DateTimeOffset.Now;
        _lastActionTime = _simulationStartTime;
        _logger.LogInformation("Simulation has started");
        while (cancellationToken is { IsCancellationRequested: false } or null)
        {
            // TODO Receive messages
            var messages = await _messageService.GetMessagesAsync();
            // TODO Process messages - Update state
            await _simulationMessageProcessor.ProcessAsync(this, messages);
            // TODO Perform actions
            _lastActionTime = DateTimeOffset.Now;
            // TODO Director - Random events
            // TODO Send updates
            await Task.Delay(30);
        }
    }
}