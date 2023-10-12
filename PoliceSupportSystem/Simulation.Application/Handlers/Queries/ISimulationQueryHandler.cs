using Simulation.Communication.Messages;
using Simulation.Communication.Queries;

namespace Simulation.Application.Handlers.Queries;

public interface ISimulationQueryHandler<in TMessage, TResult> where TMessage : ISimulationQuery<TResult> where TResult : ISimulationMessage
{
    Task<TResult> HandleQueryAsync(ISimulation simulation, TMessage message);
}