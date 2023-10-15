using Microsoft.Extensions.Logging;
using Shared.CommonTypes.Patrol;
using Simulation.Application.Entities.Patrol.Orders;
using Simulation.Communication.Messages;

namespace Simulation.Application.Handlers.Messages;

internal class PatrolDistrictMessageHandler : BaseSimulationMessageHandler<PatrolDistrictMessage>
{
    private readonly ILogger<PatrolDistrictMessageHandler> _logger;

    public PatrolDistrictMessageHandler(ILogger<PatrolDistrictMessageHandler> logger)
    {
        _logger = logger;
    }

    public override async Task HandleAsync(ISimulation simulation, PatrolDistrictMessage message)
    {
        var patrol = simulation.Patrols.FirstOrDefault(x => x.PatrolId.Equals(message.PatrolId, StringComparison.InvariantCultureIgnoreCase)) ??
                     throw new Exception($"Patrol with ID {message.PatrolId} not found");
        
        patrol.UpdateStatus(PatrolStatusEnum.Patrolling);
        patrol.Order = new PatrollingOrder(message.DistrictName);
    }
}