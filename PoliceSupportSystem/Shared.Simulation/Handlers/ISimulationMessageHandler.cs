using Simulation.Shared.Communication;

namespace Shared.Simulation.Handlers;

public interface ISimulationMessageHandler<TSimulationMessage> where TSimulationMessage : ISimulationMessage
{
    Task Handle(TSimulationMessage simulationMessage);
}