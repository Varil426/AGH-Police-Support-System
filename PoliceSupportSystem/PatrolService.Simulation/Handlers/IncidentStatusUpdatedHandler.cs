using Microsoft.Extensions.Logging;
using PatrolService.Application;
using Shared.Application.Agents.Communication.Signals;
using Shared.Simulation.Handlers;
using Simulation.Communication.Messages;

namespace PatrolService.Simulation.Handlers;

internal class IncidentStatusUpdatedHandler : ISimulationMessageHandler<IncidentStatusUpdatedMessage>
{
    private readonly ILogger<IncidentStatusUpdatedHandler> _logger;
    private readonly PatrolAgent _patrolAgent;

    public IncidentStatusUpdatedHandler(ILogger<IncidentStatusUpdatedHandler> logger, PatrolAgent patrolAgent)
    {
        _logger = logger;
        _patrolAgent = patrolAgent;
    }

    public async Task Handle(IncidentStatusUpdatedMessage simulationMessage) => await _patrolAgent.PushEnvironmentSignalAsync(new IncidentResolvedSignal(simulationMessage.IncidentId));
}