using Simulation.Application.Services;
using Simulation.Communication.Messages;
using Simulation.Communication.Queries;

namespace Simulation.Application.Handlers.Queries;

internal class GetDistrictsQueryHandler : ISimulationQueryHandler<GetDistrictsQuery, ListDistrictsMessage>
{
    private readonly IMapService _mapService;

    public GetDistrictsQueryHandler(IMapService mapService)
    {
        _mapService = mapService;
    }

    public async Task<ListDistrictsMessage> HandleQueryAsync(ISimulation simulation, GetDistrictsQuery message) => new(await _mapService.GetDistrictNames());
}