using NavigationService.Application.Services;
using Shared.CommonTypes.Geo;

namespace NavigationService.Simulation.Services;

public class SimulationNavigationService : INavigationService
{
    internal Position? LastKnowPosition { private get; set; }

    public async Task<Position> GetCurrentPosition()
    {
        while (LastKnowPosition == null) 
            await Task.Delay(TimeSpan.FromMilliseconds(50));

        return LastKnowPosition;
    }
}