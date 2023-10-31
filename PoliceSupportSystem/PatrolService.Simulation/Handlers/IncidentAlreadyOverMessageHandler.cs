using PatrolService.Application;
using Shared.Application.Agents.Communication.Signals;
using Shared.Simulation.Handlers;
using Simulation.Communication.Messages;

namespace PatrolService.Simulation.Handlers;

internal class IncidentAlreadyOverMessageHandler : ISimulationMessageHandler<IncidentAlreadyOverMessage>
{
    private readonly PatrolAgent _patrolAgent;

    public IncidentAlreadyOverMessageHandler(PatrolAgent patrolAgent)
    {
        _patrolAgent = patrolAgent;
    }

    public Task Handle(IncidentAlreadyOverMessage simulationMessage) => _patrolAgent.PushEnvironmentSignalAsync(new IncidentAlreadyOverSignal(simulationMessage.IncidentId));
}