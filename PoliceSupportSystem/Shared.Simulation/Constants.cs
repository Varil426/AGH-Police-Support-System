using Shared.Simulation.Services;

namespace Shared.Simulation;

internal record Constants
{
    public const string SimulationBusKey = "SimulationBus";
    public const string SimulationSubscriberErrorServiceKey = nameof(SimulationSubscriberErrorService);
}