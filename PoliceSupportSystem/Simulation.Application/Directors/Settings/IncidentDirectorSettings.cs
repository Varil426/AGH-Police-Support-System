using Simulation.Application.Entities;

namespace Simulation.Application.Directors.Settings;

public record IncidentDirectorSettings()
{
    public IDictionary<string, DistrictDangerLevelEnum> DistrictDangerLevels { get; init; }
}