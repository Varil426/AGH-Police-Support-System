using NavigationService.Application;
using NavigationService.Simulation.Services;
using Shared.Application.Agents.Communication.Signals;
using Shared.Simulation.Handlers;
using Simulation.Communication.Messages;

namespace NavigationService.Simulation.Handlers;

internal class PatrolPositionChangedMessageHandler : ISimulationMessageHandler<PatrolPositionChangedMessage>
{
    private readonly NavigationAgent _navigationAgent;
    private readonly SimulationNavigationService _simulationNavigationService;

    // TODO This can be improved
    private static DateTimeOffset _lastUpdate = DateTimeOffset.MinValue;

    public PatrolPositionChangedMessageHandler(NavigationAgent navigationAgent, SimulationNavigationService simulationNavigationService)
    {
        _navigationAgent = navigationAgent;
        _simulationNavigationService = simulationNavigationService;
    }

    public async Task Handle(PatrolPositionChangedMessage simulationMessage)
    {
        if (simulationMessage.CreatedAt < _lastUpdate.AddSeconds(0.5))
            return;
        _simulationNavigationService.LastKnowPosition = simulationMessage.NewPosition;
        await _navigationAgent.PushEnvironmentSignalAsync(new PositionChangedSignal(simulationMessage.NewPosition));
        _lastUpdate = simulationMessage.CreatedAt;
    }
}