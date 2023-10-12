using NavigationService.Application.Services;
using Shared.Application.Services;
using Shared.CommonTypes.Geo;
using Shared.Simulation.Services;
using Simulation.Communication.Messages;

namespace NavigationService.Simulation.Services;

public class SimulationNavigationService : INavigationService
{
    private readonly ISimulationMessageBus _messageBus;
    private readonly IPatrolInfoService _patrolInfoService;
    internal Position? LastKnowPosition { private get; set; }

    public SimulationNavigationService(ISimulationMessageBus messageBus, IPatrolInfoService patrolInfoService)
    {
        _messageBus = messageBus;
        _patrolInfoService = patrolInfoService;
    }

    public async Task<Position> GetCurrentPosition()
    {
        while (LastKnowPosition == null) 
            await Task.Delay(TimeSpan.FromMilliseconds(50));

        return LastKnowPosition;
    }

    public Task DisplayDistrict(string districtName) => _messageBus.SendSimulationMessage(new PatrolDistrictMessage(districtName, _patrolInfoService.PatrolId));
}