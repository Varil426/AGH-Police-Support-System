using Simulation.Communication.Messages;

namespace Simulation.Application.Handlers.Messages;

internal abstract class BaseSimulationMessageHandler<TMessage> : ISimulationMessageHandler<TMessage> where TMessage : class, ISimulationMessage
{
    public abstract Task HandleAsync(ISimulation simulation, TMessage message);

    public bool CanHandle(ISimulationMessage message) => message is TMessage;

    public Task HandleAsync(ISimulation simulation, ISimulationMessage message) => HandleAsync(simulation, message as TMessage ?? throw new Exception("Bad message type"));
}