using Shared.CommonTypes.District;

namespace HqService.Application.Settings;

internal record DecisionServiceSettings(bool EvenPatrolDistribution, double DistanceWeight, double SameDistrictWeight, double InsufficientNumberOfPatrolsInDistrictWeight)
{
    public required IReadOnlyDictionary<string, DistrictDangerLevelEnum> DistrictDangerLevels { get; init; }
    
    public required IReadOnlyDictionary<DistrictDangerLevelEnum, int> DangerLevelRequiredPatrollingPatrols { get; init; }
}