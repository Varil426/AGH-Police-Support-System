namespace Simulation.Communication.Messages;

public interface ISimulationMessage
{
    DateTimeOffset CreatedAt { get; }
}