namespace Simulation.Shared.Communication;

public interface ISimulationMessage
{
    DateTimeOffset CreatedAt { get; }
}