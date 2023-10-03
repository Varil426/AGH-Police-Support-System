using Simulation.Communication.Messages;

namespace Simulation.Application.Services;

public interface ISimulationMessageProcessor
{
    Task ProcessAsync(ISimulation simulation, ISimulationMessage simulationMessage);

    Task ProcessAsync(ISimulation simulation, IEnumerable<ISimulationMessage> simulationMessages);
}