using HqService.Application.Settings;
using Shared.Application.Helpers;
using Shared.Application.Integration.Events;
using Shared.Application.Services;
using Shared.CommonTypes.District;
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
    private readonly IMessageBus _messageBus;
    private readonly Random _random;

    private readonly Dictionary<Guid, string> _patrolToDistrictAssignment = new();

    private readonly Dictionary<Incident, IList<Patrol>> _patrolsRelatedToIncident = new();

    public DecisionService(IMapInfoService mapInfoService, DecisionServiceSettings decisionServiceSettings, IMessageBus messageBus)
    {
        _mapInfoService = mapInfoService;
        _decisionServiceSettings = decisionServiceSettings;
        _messageBus = messageBus;
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
            await _messageBus.PublishAsync(new PatrolsDistanceToIncidentEvent(PatrolsNotOrdered().Select(x => x.Position.GetDistanceTo(incident.Location))));
            
            var chosenPatrol = PatrolsNotOrdered().Where(x => x.Status is PatrolStatusEnum.Patrolling or PatrolStatusEnum.AwaitingOrders)
                .MinBy(x => x.Position.GetDistanceTo(incident.Location));

            if (chosenPatrol is null)
                break;

            await _messageBus.PublishAsync(new ChosenPatrolDistanceToIncidentEvent(chosenPatrol.Position.GetDistanceTo(incident.Location)));
            
            orders.Add(new HandleIncidentOrder(chosenPatrol.Id, chosenPatrol.PatrolId, incident.AsDto()));
            patrolsOrdered.Add(chosenPatrol);
            AddRelatedPatrol(incident, chosenPatrol);
        }
        
        // Rest of the patrols can patrol
        orders.AddRange(
            PatrolsNotOrdered()
                .Where(x => x.Status == PatrolStatusEnum.AwaitingOrders)
                .Select(
                    x =>
                    {
                        PatrollingOrder order;
                        if (_patrolToDistrictAssignment.TryGetValue(x.Id, out var assignedDistrictName))
                            order = new PatrollingOrder(x.Id, x.PatrolId, assignedDistrictName);
                        else
                        {
                            var chosenDistrict = ChooseDistrictForPatrol(districts);
                            _patrolToDistrictAssignment[x.Id] = chosenDistrict;
                            order = new PatrollingOrder(x.Id, x.PatrolId, chosenDistrict);
                        }
                        
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

    private string ChooseDistrictForPatrol(IList<string> districts)
    {
        string chosenDistrict;
        if (_decisionServiceSettings.EvenPatrolDistribution)
        {
            var numberOfPatrolsAssignedToDistrict = districts.ToDictionary(x => x, x => 0);
            foreach (var group in _patrolToDistrictAssignment.Values.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count()))
                numberOfPatrolsAssignedToDistrict[group.Key] += group.Value;
            
            var districtsWithLowestNumberOfPatrols = numberOfPatrolsAssignedToDistrict.GroupBy(x => x.Value).OrderBy(x => x.Key).First().Select(x => x.Key).ToList();
            
            chosenDistrict = districtsWithLowestNumberOfPatrols[_random.Next(districtsWithLowestNumberOfPatrols.Count)];
        }
        else
        {
            var numberOfPatrolsAssignedToDistrict = districts.ToDictionary(x => x, x => 0);
            foreach (var group in _patrolToDistrictAssignment.Values.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count()))
                numberOfPatrolsAssignedToDistrict[group.Key] += group.Value;

            var missingPatrolsForDistrict = numberOfPatrolsAssignedToDistrict.ToDictionary(
                x => x.Key,
                x => x.Value - GetOptimalNumberOfPatrolsForDangerLevel(GetDistrictDangerLevel(x.Key)));
            
            var districtsThatMostNeedPatrol = missingPatrolsForDistrict.GroupBy(x => x.Value).OrderBy(x => x.Key).First().Select(x => x.Key).ToList();
            
            chosenDistrict = districtsThatMostNeedPatrol[_random.Next(districtsThatMostNeedPatrol.Count)];;
        }

        return chosenDistrict;
    }
    
    private int GetNumberOfRelatedPatrols(Incident incident) => _patrolsRelatedToIncident.TryGetValue(incident, out var l) ? l.Count : 0;

    private DistrictDangerLevelEnum GetDistrictDangerLevel(string districtName) => _decisionServiceSettings.DistrictDangerLevels.TryGetValue(districtName, out var dangerLevelEnum)
        ? dangerLevelEnum
        : DistrictDangerLevelEnum.Normal;

    private int GetOptimalNumberOfPatrolsForDangerLevel(DistrictDangerLevelEnum dangerLevelEnum) => _decisionServiceSettings.DangerLevelRequiredPatrollingPatrols[dangerLevelEnum];
}