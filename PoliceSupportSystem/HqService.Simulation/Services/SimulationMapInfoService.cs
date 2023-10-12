using HqService.Application.Services;
using Shared.Simulation.Services;
using Simulation.Communication.Messages;
using Simulation.Communication.Queries;

namespace HqService.Simulation.Services;

internal class SimulationMapInfoService : IMapInfoService
{
    private readonly ISimulationMessageBus _simulationMessageBus;
    private IList<string>? _districts;

    public SimulationMapInfoService(ISimulationMessageBus simulationMessageBus)
    {
        _simulationMessageBus = simulationMessageBus;
    }

    public async Task<IList<string>> GetDistricts()
    {
        _districts ??= (await _simulationMessageBus.QuerySimulationMessage<GetDistrictsQuery, ListDistrictsMessage>(new GetDistrictsQuery())).Districts.ToList();
        return _districts;
    }
}