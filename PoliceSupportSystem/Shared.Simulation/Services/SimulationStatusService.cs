using Microsoft.Extensions.Logging;
using Shared.Application.Services;
using Simulation.Communication.Messages;

namespace Shared.Simulation.Services;

internal class SimulationStatusService : IStatusService
{
    private readonly ILogger<SimulationStatusService> _logger;
    private readonly ISimulationMessageBus _simulationMessageBus;
    
    protected IServiceInfoService ServiceInfoService { get; }
    protected ITypeMapper TypeMapper { get; }

    public SimulationStatusService(
        ILogger<SimulationStatusService> logger,
        ISimulationMessageBus simulationMessageBus,
        IServiceInfoService serviceInfoService,
        ITypeMapper typeMapper)
    {
        _logger = logger;
        _simulationMessageBus = simulationMessageBus;
        ServiceInfoService = serviceInfoService;
        TypeMapper = typeMapper;
    }
    
    public async Task AnnounceOnline()
    {
        _logger.LogInformation($"Service: {ServiceInfoService.Id} of type: {ServiceInfoService.ServiceType} is online and ready.");
        await _simulationMessageBus.SendSimulationMessage(PrepareOnlineMessage());
    }

    public async Task AnnounceOffline()
    {
        _logger.LogInformation($"Service: {ServiceInfoService.Id} goes offline.");
        await _simulationMessageBus.SendSimulationMessage(new ServiceOfflineMessage(ServiceInfoService.Id));
    }

    protected virtual ISimulationMessage PrepareOnlineMessage() => new ServiceOnlineMessage(ServiceInfoService.Id, TypeMapper.Map(ServiceInfoService.ServiceType));
}