using Shared.CommonTypes.Patrol;
using Shared.Domain.Incident;
using Shared.Domain.Patrol;

namespace HqService.Application.Services.DecisionService;

internal class SimpleDecisionService : IDecisionService
{
    private readonly IMapInfoService _mapInfoService;
    private readonly Random _random;

    public SimpleDecisionService(IMapInfoService mapInfoService)
    {
        _mapInfoService = mapInfoService;
        _random = new Random();
    }

    public async Task<IReadOnlyList<PatrolOrder>> ComputeOrders(IEnumerable<Incident> onGoingIncidents, IEnumerable<Patrol> patrols)
    {
        var districts = await _mapInfoService.GetDistricts();
        var orders = new List<PatrolOrder>();
        
        // TODO Handle incidents
        
        // Rest of the patrols can patrol
        // TODO Improve selection? Add some balancing?
        orders.AddRange(
            patrols.Where(x => x.Status == PatrolStatusEnum.AwaitingOrders)
                .Select(
                    x => new PatrollingOrder(
                        x.Id,
                        x.PatrolId,
                        districts[_random.Next(districts.Count)])));

        return orders;
    }
}