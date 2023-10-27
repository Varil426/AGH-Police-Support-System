using Microsoft.Extensions.Logging;
using NavigationService.Application;
using Shared.Application.Agents.Communication.Signals;
using Shared.Simulation.Handlers;
using Simulation.Communication.Messages;

namespace NavigationService.Simulation.Handlers;

internal class DestinationReachedMessageHandler : ISimulationMessageHandler<DestinationReachedMessage>
{
    private readonly ILogger<DestinationReachedMessageHandler> _logger;
    private readonly NavigationAgent _navigationAgent;

    public DestinationReachedMessageHandler(ILogger<DestinationReachedMessageHandler> logger, NavigationAgent navigationAgent)
    {
        _logger = logger;
        _navigationAgent = navigationAgent;
    }

    public async Task Handle(DestinationReachedMessage simulationMessage)
    {
        _logger.LogInformation("Destination reached.");
        await _navigationAgent.PushEnvironmentSignalAsync(new DestinationReachedSignal());
    }
}