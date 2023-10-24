using Shared.Application.Helpers;
using Shared.CommonTypes.Incident;
using Shared.CommonTypes.Patrol;
using Shared.Domain.Helpers;
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

    public async Task<IReadOnlyList<PatrolOrder>> ComputeOrders(IEnumerable<Incident> onGoingIncidents, IReadOnlyCollection<Patrol> patrols)
    {
        var districts = await _mapInfoService.GetDistricts();
        var orders = new List<PatrolOrder>();
        var patrolsOrdered = new List<Patrol>();

        IReadOnlyCollection<Patrol> PatrolsNotOrdered() => patrols.Where(x => !patrolsOrdered.Contains(x)).ToList();
        
        // TODO Handle incidents
        foreach (var incident in onGoingIncidents.Where(x => x.Status == IncidentStatusEnum.WaitingForResponse))
        {
            var closestFreePatrol = PatrolsNotOrdered().Where(x => x.Status is PatrolStatusEnum.Patrolling or PatrolStatusEnum.AwaitingOrders)
                .MinBy(x => x.Position.GetDistanceTo(incident.Location));

            if (closestFreePatrol is null)
                break;
            
            orders.Add(new HandleIncidentOrder(closestFreePatrol.Id, closestFreePatrol.PatrolId, incident.AsDto()));
            patrolsOrdered.Add(closestFreePatrol);
        }
        
        // Rest of the patrols can patrol
        // TODO Improve selection? Add some balancing?
        orders.AddRange(
            PatrolsNotOrdered()
                .Where(x => x.Status == PatrolStatusEnum.AwaitingOrders)
                .Select(
                    x =>
                    {
                        var order = new PatrollingOrder(
                            x.Id,
                            x.PatrolId,
                            districts[_random.Next(districts.Count)]);
                        patrolsOrdered.Add(x);
                        return order;
                    }));

        return orders;
    }
}