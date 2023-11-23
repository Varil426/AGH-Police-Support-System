using HqService.Application.Settings;
using Shared.Application.Helpers;
using Shared.CommonTypes.Incident;
using Shared.CommonTypes.Patrol;
using Shared.Domain.Helpers;
using Shared.Domain.Incident;
using Shared.Domain.Patrol;

namespace HqService.Application.Services.DecisionService;

internal class DecisionService : IDecisionService
{
    private const int MaxNumberOfSupportingPatrols = 1;
    private readonly IMapInfoService _mapInfoService;
    private readonly DecisionServiceSettings _decisionServiceSettings;
    private readonly Random _random;

    private readonly Dictionary<Incident, IList<Patrol>> _patrolsRelatedToIncident = new();

    public DecisionService(IMapInfoService mapInfoService, DecisionServiceSettings decisionServiceSettings)
    {
        _mapInfoService = mapInfoService;
        _decisionServiceSettings = decisionServiceSettings;
        _random = new Random();
    }

    public async Task<IReadOnlyList<PatrolOrder>> ComputeOrders(IEnumerable<Incident> onGoingIncidents, IReadOnlyCollection<Patrol> patrols)
    {
        RefreshRelatedList();
        var districts = await _mapInfoService.GetDistricts();
        var orders = new List<PatrolOrder>();
        var patrolsOrdered = new List<Patrol>();

        IReadOnlyCollection<Patrol> PatrolsNotOrdered() => patrols.Where(x => !patrolsOrdered.Contains(x)).ToList();
        
        var onGoingIncidentsList = onGoingIncidents.ToList();
        foreach (var shooting in onGoingIncidentsList.Where(x => x.Status == IncidentStatusEnum.OnGoingShooting))
        {
            if (GetNumberOfRelatedPatrols(shooting) >= MaxNumberOfSupportingPatrols + 1)
                continue;
            
            var closestFreePatrols = PatrolsNotOrdered().Where(x => x.Status is PatrolStatusEnum.Patrolling or PatrolStatusEnum.AwaitingOrders)
                .OrderBy(x => x.Position.GetDistanceTo(shooting.Location)).Take(MaxNumberOfSupportingPatrols);

            foreach (var patrol in closestFreePatrols)
            {
                orders.Add(new SupportShootingOrder(patrol.Id, patrol.PatrolId, shooting.AsDto()));
                patrolsOrdered.Add(patrol);
                AddRelatedPatrol(shooting, patrol);
            }
        }
        
        foreach (var incident in onGoingIncidentsList.Where(x => x.Status == IncidentStatusEnum.WaitingForResponse))
        {
            var closestFreePatrol = PatrolsNotOrdered().Where(x => x.Status is PatrolStatusEnum.Patrolling or PatrolStatusEnum.AwaitingOrders)
                .MinBy(x => x.Position.GetDistanceTo(incident.Location));

            if (closestFreePatrol is null)
                break;
            
            orders.Add(new HandleIncidentOrder(closestFreePatrol.Id, closestFreePatrol.PatrolId, incident.AsDto()));
            patrolsOrdered.Add(closestFreePatrol);
            AddRelatedPatrol(incident, closestFreePatrol);
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

    private void RefreshRelatedList()
    {
        var resolved = _patrolsRelatedToIncident.Keys.Where(x => x.Status == IncidentStatusEnum.Resolved).ToList();
        resolved.ForEach(x => _patrolsRelatedToIncident.Remove(x));
    }
    
    private void AddRelatedPatrol(Incident incident, Patrol patrol)
    {
        if (!_patrolsRelatedToIncident.ContainsKey(incident))
            _patrolsRelatedToIncident[incident] = new List<Patrol>();
        
        _patrolsRelatedToIncident[incident].Add(patrol);
    }

    private int GetNumberOfRelatedPatrols(Incident incident) => _patrolsRelatedToIncident.TryGetValue(incident, out var l) ? l.Count : 0;
}