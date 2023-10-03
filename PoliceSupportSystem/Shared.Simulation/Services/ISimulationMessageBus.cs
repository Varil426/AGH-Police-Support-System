using Simulation.Communication.Messages;

namespace Shared.Simulation.Services;

public interface ISimulationMessageBus
{
    Task SendSimulationMessage(ISimulationMessage simulationMessage);
}