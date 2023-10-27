using PatrolService.Application.Services;
using Shared.Application.Services;
using Shared.Simulation.Services;
using Simulation.Communication.Messages;

namespace PatrolService.Simulation.Services;

internal class ConfirmationService : IConfirmationService
{
    private readonly ISimulationMessageBus _messageBus;
    private readonly IPatrolInfoService _patrolInfoService;

    public ConfirmationService(ISimulationMessageBus messageBus, IPatrolInfoService patrolInfoService)
    {
        _messageBus = messageBus;
        _patrolInfoService = patrolInfoService;
    }

    public async Task ConfirmIncidentStart(Guid incidentId)
        => await _messageBus.SendSimulationMessage(new PatrolConfirmIncidentStartMessage(_patrolInfoService.PatrolId, incidentId));
}