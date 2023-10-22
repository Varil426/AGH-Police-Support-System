using Microsoft.Extensions.Logging;
using NavigationService.Simulation.Services;
using Shared.Application.Services;
using Shared.Simulation.Handlers;
using Simulation.Communication.Messages;

namespace NavigationService.Simulation.Handlers;

public class CurrentPositionMessageHandler : ISimulationMessageHandler<CurrentPositionMessage>
{
    private readonly SimulationNavigationService _simulationNavigationService;
    private readonly ILogger<CurrentPositionMessageHandler> _logger;
    private readonly IServiceInfoService _serviceInfoService;

    public CurrentPositionMessageHandler(
        SimulationNavigationService simulationNavigationService,
        ILogger<CurrentPositionMessageHandler> logger,
        IServiceInfoService serviceInfoService)
    {
        _simulationNavigationService = simulationNavigationService;
        _logger = logger;
        _serviceInfoService = serviceInfoService;
    }

    public Task Handle(CurrentPositionMessage simulationMessage)
    {
        if (simulationMessage.Receiver != _serviceInfoService.Id)
            _logger.LogWarning("Received message for a different receiver.");

        _simulationNavigationService.LastKnowPosition = simulationMessage.Position;
        return Task.CompletedTask;
    }
}