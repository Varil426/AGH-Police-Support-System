﻿namespace Shared.Infrastructure.Settings;

public record RabbitMqSettings(
    string Host,
    ushort Port,
    string? Username = null,
    string? Password = null,
    string? MessageExchange = null,
    string? DirectMessageExchange = null,
    string? EventExchange = null,
    string? QueryExchange = null,
    string? CommandExchange = null);