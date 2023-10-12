using Simulation.Communication.Messages;

namespace Simulation.Communication.Queries;

public interface ISimulationQuery<TResultType> /*: ISimulationMessage*/ where TResultType : ISimulationMessage
{
}