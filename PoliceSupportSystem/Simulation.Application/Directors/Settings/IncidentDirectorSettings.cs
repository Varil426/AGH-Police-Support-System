using Shared.CommonTypes.District;

namespace Simulation.Application.Directors.Settings;

public record IncidentDirectorSettings()
{
    public required IReadOnlyDictionary<string, DistrictDangerLevelEnum> DistrictDangerLevels { get; init; }
    public required IReadOnlyDictionary<DistrictDangerLevelEnum, double> DangerLevelShootingChance { get; init; }
    public required IReadOnlyDictionary<DistrictDangerLevelEnum, int> DangerLevelMaxNumberOfIncidentPerDay { get; init; }
}