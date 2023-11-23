using Simulation.Application.Services;
using Simulation.Communication.Messages;
using Simulation.Communication.Queries;

namespace Simulation.Application.Handlers.Queries;

internal class GetDistrictQueryHandler : ISimulationQueryHandler<GetDistrictQuery, DistrictNameMessage>
{
    private readonly IMapService _mapService;

    public GetDistrictQueryHandler(IMapService mapService)
    {
        _mapService = mapService;
    }

    public async Task<DistrictNameMessage> HandleQueryAsync(ISimulation simulation, GetDistrictQuery message) => new(await _mapService.GetDistrictName(message.Position));
}