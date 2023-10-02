using Shared.CommonTypes.Geo;

namespace Simulation.Application;

public record SimulationSettings(double TimeRate, TimeSpan StartDelay = default)
{
    public required Position HqLocation { get; init; }
}