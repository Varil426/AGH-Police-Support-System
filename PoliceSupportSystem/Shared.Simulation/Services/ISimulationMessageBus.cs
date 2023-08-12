using Simulation.Shared.Communication;

namespace Shared.Simulation.Services;

public interface ISimulationMessageBus
{
    Task SendSimulationMessage(ISimulationMessage simulationMessage);
}