using Simulation.Communication.Messages;

namespace Simulation.Application.Handlers;

public interface ISimulationMessageHandler
{
    bool CanHandle(ISimulationMessage message);

    Task HandleAsync(ISimulation simulation, ISimulationMessage message);
}

public interface ISimulationMessageHandler<in TMessage> : ISimulationMessageHandler where TMessage : ISimulationMessage
{
    Task HandleAsync(ISimulation simulation, TMessage message);
}