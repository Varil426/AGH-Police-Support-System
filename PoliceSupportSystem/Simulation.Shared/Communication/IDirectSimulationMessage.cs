namespace Simulation.Shared.Communication;

public interface IDirectSimulationMessage : ISimulationMessage
{
     string Receiver { get; init; }
}