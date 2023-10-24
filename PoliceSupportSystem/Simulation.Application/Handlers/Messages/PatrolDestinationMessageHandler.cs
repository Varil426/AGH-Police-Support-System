using Microsoft.Extensions.Logging;
using Shared.CommonTypes.Patrol;
using Simulation.Application.Entities.Patrol.Orders;
using Simulation.Communication.Messages;

namespace Simulation.Application.Handlers.Messages;

internal class PatrolDestinationMessageHandler : BaseSimulationMessageHandler<PatrolDestinationMessage>
{
    private readonly ILogger<PatrolDestinationMessageHandler> _logger;

    public PatrolDestinationMessageHandler(ILogger<PatrolDestinationMessageHandler> logger)
    {
        _logger = logger;
    }
    
    public override Task HandleAsync(ISimulation simulation, PatrolDestinationMessage message)
    {
        var patrol = simulation.Patrols.FirstOrDefault(x => x.PatrolId.Equals(message.PatrolId, StringComparison.InvariantCultureIgnoreCase)) ??
                     throw new Exception($"Patrol with ID {message.PatrolId} not found");
        
        patrol.UpdateStatus(PatrolStatusEnum.ResolvingIncident);
        patrol.Order = new MoveOrder(message.Position);
        return Task.CompletedTask;
    }
}