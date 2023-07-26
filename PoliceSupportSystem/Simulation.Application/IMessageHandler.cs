using Simulation.Shared.Communication;

namespace Simulation.Application;

public interface IMessageHandler<TMessage> where TMessage : ISimulationMessage
{
    Task HandleAsync(TMessage message);
}