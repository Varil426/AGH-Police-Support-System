using Simulation.Communication.Messages;
using Simulation.Communication.Queries;

namespace Shared.Simulation.Services;

public interface ISimulationMessageBus
{
    Task SendSimulationMessage(ISimulationMessage simulationMessage);

    Task<TResultType> QuerySimulationMessage<TQueryType, TResultType>(
        TQueryType query) where TQueryType : ISimulationQuery<TResultType>
        where TResultType : ISimulationMessage;
}