namespace Simulation.Shared.Communication;

public record ServiceOfflineMessage(string ServiceId) : ISimulationMessage;