using HqService.Application.Services;
using Shared.CommonTypes.Geo;
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

    public async Task<string> GetDistrict(Position position) =>
        (await _simulationMessageBus.QuerySimulationMessage<GetDistrictQuery, DistrictNameMessage>(new GetDistrictQuery(position))).DistrictName ??
        throw new Exception("Unknown district");
}