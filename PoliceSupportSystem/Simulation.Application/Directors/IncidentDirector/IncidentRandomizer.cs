using Shared.CommonTypes.District;
using Shared.CommonTypes.Incident;
using Simulation.Application.Directors.Settings;
using Simulation.Application.Entities;
using Simulation.Application.Helpers;
using Simulation.Application.Services;

namespace Simulation.Application.Directors.IncidentDirector;

internal class IncidentRandomizer : IIncidentRandomizer
{
    private const int MinimumIncidentDuration = 30;
    private const int MaximumIncidentDuration = 120;

    private const int MinimumTimeToChangeIntoShooting = 1;
    private const int MaximumTimeToChangeIntoShooting = 30;

    private readonly IncidentDirectorSettings _settings;
    private readonly IMapService _mapService;
    private readonly Random _random = new();

    public IncidentRandomizer(IncidentDirectorSettings settings, IMapService mapService)
    {
        _settings = settings;
        _mapService = mapService;
    }

    public int DetermineNumberOfIncidentsForDay(DistrictDangerLevelEnum dangerLevel) => _random.Next(GetMaxNumberOfIncidentPerDay(dangerLevel));

    public bool ShouldChangeIntoShooting(DistrictDangerLevelEnum dangerLevel) => _random.NextDouble() <= GetChanceToChangeIntoShooting(dangerLevel);

    public async Task<IEnumerable<PlannedIncident>> PlanIncidents(District district, TimeSpan currentSimulationTime, TimeSpan planAheadFor)
    {
        var results = new List<PlannedIncident>();
        var numberOfIncidents = (int)Math.Floor(DetermineNumberOfIncidentsForDay(district.DangerLevel) * planAheadFor.TotalDays);
        var positions = await _mapService.GetRandomPositionsInDistrict(district.Name, numberOfIncidents);

        foreach (var position in positions)
        {
            var startAfter = currentSimulationTime + TimeSpan.FromMinutes(_random.Next((int)planAheadFor.TotalMinutes));
            var finishAfter = TimeSpan.FromMinutes(_random.Next(MinimumIncidentDuration, MaximumIncidentDuration));
            TimeSpan? changeIntoShootingAfter = ShouldChangeIntoShooting(district.DangerLevel) ? TimeSpan.FromMinutes(_random.Next(MinimumTimeToChangeIntoShooting, MaximumTimeToChangeIntoShooting)) : null;

            results.Add(new PlannedIncident(Guid.NewGuid(), position, startAfter, IncidentTypeEnum.NormalIncident, finishAfter, changeIntoShootingAfter, changeIntoShootingAfter is null ? null : 2)); // TODO Random number of required patrols to solve
        }

        return results;
    }

    private int GetMaxNumberOfIncidentPerDay(DistrictDangerLevelEnum dangerLevel) => _settings.DangerLevelMaxNumberOfIncidentPerDay.TryGetNullable(dangerLevel) ??
                                                                                     dangerLevel switch
                                                                                     {
                                                                                         DistrictDangerLevelEnum.Low => 3,
                                                                                         DistrictDangerLevelEnum.High => 15,
                                                                                         DistrictDangerLevelEnum.Normal or _ => 8,
                                                                                     };

    private double GetChanceToChangeIntoShooting(DistrictDangerLevelEnum dangerLevel) => _settings.DangerLevelShootingChance.TryGetNullable(dangerLevel) ??
                                                                                        dangerLevel switch
                                                                                        {
                                                                                            DistrictDangerLevelEnum.Low => 0,
                                                                                            DistrictDangerLevelEnum.High => 0.2,
                                                                                            DistrictDangerLevelEnum.Normal or _ => 0.05
                                                                                        };
}