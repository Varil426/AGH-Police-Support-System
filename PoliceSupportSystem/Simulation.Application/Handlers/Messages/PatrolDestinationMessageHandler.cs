using Microsoft.Extensions.Logging;
using Simulation.Application.Directors.PatrolDirector;
using Simulation.Application.Entities.Patrol.Actions;
using Simulation.Communication.Messages;

namespace Simulation.Application.Handlers.Messages;

internal class PatrolDestinationMessageHandler : BaseSimulationMessageHandler<PatrolDestinationMessage>
{
    private readonly ILogger<PatrolDestinationMessageHandler> _logger;
    private readonly IRouteBuilder _routeBuilder;

    public PatrolDestinationMessageHandler(ILogger<PatrolDestinationMessageHandler> logger, IRouteBuilder routeBuilder)
    {
        _logger = logger;
        _routeBuilder = routeBuilder;
    }
    
    public override async Task HandleAsync(ISimulation simulation, PatrolDestinationMessage message)
    {
        var patrol = simulation.Patrols.FirstOrDefault(x => x.PatrolId.Equals(message.PatrolId, StringComparison.InvariantCultureIgnoreCase)) ??
                     throw new Exception($"Patrol with ID {message.PatrolId} not found");

        var route = await _routeBuilder.CreateRoute(patrol.Position, message.Position);
        patrol.Action = new NavigatingAction(route);
    }
}