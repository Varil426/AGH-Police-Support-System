namespace Simulation.Infrastructure.Settings;

public record RabbitMqSettings(
    string Host,
    ushort Port,
    string? Username = null,
    string? Password = null,
    string? IncomingMessageQueueName = "SimulationIncomingMessages",
    string? SimulationExchangeName = "SimulationExchange");