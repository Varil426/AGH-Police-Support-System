namespace Simulation.Communication.Messages;

public interface IDirectSimulationMessage : ISimulationMessage
{
     string Receiver { get; init; }
}