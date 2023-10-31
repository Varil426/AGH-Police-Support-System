using Microsoft.Extensions.Logging;
using PatrolService.Application;
using Shared.Application.Agents.Communication.Signals;
using Shared.CommonTypes.Incident;
using Shared.Simulation.Handlers;
using Simulation.Communication.Messages;

namespace PatrolService.Simulation.Handlers;

internal class IncidentStatusUpdatedMessageHandler : ISimulationMessageHandler<IncidentStatusUpdatedMessage>
{
    private readonly ILogger<IncidentStatusUpdatedMessageHandler> _logger;
    private readonly PatrolAgent _patrolAgent;

    public IncidentStatusUpdatedMessageHandler(ILogger<IncidentStatusUpdatedMessageHandler> logger, PatrolAgent patrolAgent)
    {
        _logger = logger;
        _patrolAgent = patrolAgent;
    }

    public async Task Handle(IncidentStatusUpdatedMessage simulationMessage)
    {
        switch (simulationMessage.NewStatus)
        {
            case IncidentStatusEnum.Resolved:
                await _patrolAgent.PushEnvironmentSignalAsync(new IncidentResolvedSignal(simulationMessage.IncidentId));
                break;
        }
    }
}