﻿namespace Simulation.Communication.Messages;

public record PatrolJoinedShootingMessage(string PatrolId, Guid IncidentId) : ISimulationMessage
{
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}