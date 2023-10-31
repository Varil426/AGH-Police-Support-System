using PatrolService.Application.Services;
using Shared.Application.Services;
using Shared.Simulation.Services;
using Simulation.Communication.Messages;

namespace PatrolService.Simulation.Services;

internal class SimulationConfirmationService : IConfirmationService
{
    private readonly ISimulationMessageBus _messageBus;
    private readonly IPatrolInfoService _patrolInfoService;

    public SimulationConfirmationService(ISimulationMessageBus messageBus, IPatrolInfoService patrolInfoService)
    {
        _messageBus = messageBus;
        _patrolInfoService = patrolInfoService;
    }

    public async Task ConfirmIncidentStart(Guid incidentId)
        => await _messageBus.SendSimulationMessage(new PatrolConfirmIncidentStartMessage(_patrolInfoService.PatrolId, incidentId));

    public async Task ConfirmSupportShooting(Guid incidentId)
        => await _messageBus.SendSimulationMessage(new PatrolJoinedShootingMessage(_patrolInfoService.PatrolId, incidentId));
}