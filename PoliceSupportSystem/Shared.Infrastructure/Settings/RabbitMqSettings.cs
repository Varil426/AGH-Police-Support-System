namespace Shared.Infrastructure.Settings;

public record RabbitMqSettings(
    string Host,
    int Port,
    string? Username = null,
    string? Password = null,
    string? MessageExchange = null,
    string? EventExchange = null,
    string? QueryExchange = null,
    string? CommandExchange = null);