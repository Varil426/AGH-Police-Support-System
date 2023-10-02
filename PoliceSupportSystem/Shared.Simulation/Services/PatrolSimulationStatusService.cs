using Microsoft.Extensions.Logging;
using Shared.Application.Services;
using Simulation.Shared.Communication;

namespace Shared.Simulation.Services;

internal class PatrolSimulationStatusService : SimulationStatusService
{
    private readonly IPatrolInfoService _patrolInfoService;

    public PatrolSimulationStatusService(
        ILogger<SimulationStatusService> logger,
        ISimulationMessageBus simulationMessageBus,
        IServiceInfoService serviceInfoService,
        ITypeMapper typeMapper,
        IPatrolInfoService patrolInfoService) : base(logger, simulationMessageBus, serviceInfoService, typeMapper)
    {
        _patrolInfoService = patrolInfoService;
    }

    protected override ISimulationMessage PrepareOnlineMessage() => new PatrolServiceOnline(ServiceInfoService.Id, TypeMapper.Map(ServiceInfoService.ServiceType), _patrolInfoService.PatrolId);
}