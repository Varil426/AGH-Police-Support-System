namespace Shared.Infrastructure.Settings;

public record RabbitMqSettings(
    string Host,
    int Port,
    string Exchange,
    string? Username = null,
    string? Password = null,
    string? MessageQueue = null,
    string? QueryQueue = null,
    string? CommandQueue = null,
    string? EventQueue = null);