namespace Shared.Simulation.Settings;

internal record SimulationCommunicationSettings(
    string Host,
    ushort Port,
    string? Username = null,
    string? Password = null,
    string SimulationExchangeName = "SimulationExchange",
    string SimulationQueueName = "SimulationIncomingMessages");