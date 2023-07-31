using Microsoft.Extensions.Logging;
using Shared.Application.Services;
using Simulation.Shared.Communication;

namespace Shared.Simulation.Services;

internal class SimulationStatusService : IStatusService
{
    private readonly ILogger<SimulationStatusService> _logger;
    private readonly ISimulationMessageBus _simulationMessageBus;
    private readonly IServiceInfoService _serviceInfoService;
    private readonly ITypeMapper _typeMapper;

    public SimulationStatusService(
        ILogger<SimulationStatusService> logger,
        ISimulationMessageBus simulationMessageBus,
        IServiceInfoService serviceInfoService,
        ITypeMapper typeMapper)
    {
        _logger = logger;
        _simulationMessageBus = simulationMessageBus;
        _serviceInfoService = serviceInfoService;
        _typeMapper = typeMapper;
    }

    public async Task AnnounceOnline()
    {
        _logger.LogInformation($"Service: {_serviceInfoService.Id} of type: {_serviceInfoService.ServiceType} is online and ready.");
        await _simulationMessageBus.SendSimulationMessage(new ServiceOnlineMessage(_serviceInfoService.Id, _typeMapper.Map(_serviceInfoService.ServiceType)));
    }

    public async Task AnnounceOffline()
    {
        _logger.LogInformation($"Service: {_serviceInfoService.Id} goes offline.");
        await _simulationMessageBus.SendSimulationMessage(new ServiceOfflineMessage(_serviceInfoService.Id));
    }
}