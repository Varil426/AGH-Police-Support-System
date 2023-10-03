using Simulation.Communication.Messages;

namespace Shared.Simulation.Handlers;

public interface ISimulationMessageHandler<TSimulationMessage> where TSimulationMessage : ISimulationMessage
{
    Task Handle(TSimulationMessage simulationMessage);
}