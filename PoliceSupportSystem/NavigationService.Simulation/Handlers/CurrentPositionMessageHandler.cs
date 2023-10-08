using NavigationService.Simulation.Services;
using Shared.Simulation.Handlers;
using Simulation.Communication.Messages;

namespace NavigationService.Simulation.Handlers;

public class CurrentPositionMessageHandler : ISimulationMessageHandler<CurrentPositionMessage>
{
    private readonly SimulationNavigationService _simulationNavigationService;

    public CurrentPositionMessageHandler(SimulationNavigationService simulationNavigationService)
    {
        _simulationNavigationService = simulationNavigationService;
    }

    public Task Handle(CurrentPositionMessage simulationMessage)
    {
        _simulationNavigationService.LastKnowPosition = simulationMessage.Position;
        return Task.CompletedTask;
    }
}