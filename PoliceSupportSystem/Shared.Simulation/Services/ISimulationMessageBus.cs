using Simulation.Shared.Communication;

namespace Shared.Simulation.Services;

internal interface ISimulationMessageBus
{
    Task SendSimulationMessage(ISimulationMessage simulationMessage);
}