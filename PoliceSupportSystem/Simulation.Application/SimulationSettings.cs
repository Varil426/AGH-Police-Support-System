using Shared.CommonTypes.Geo;

namespace Simulation.Application;

 public record SimulationSettings(double TimeRate, Position HqLocation, TimeSpan StartDelay = default);