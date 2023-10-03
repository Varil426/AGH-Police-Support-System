using Simulation.Communication.Common;

namespace Simulation.Communication.Messages;

public record ServiceOnlineMessage(string ServiceId, ServiceTypeEnum ServiceType) : ISimulationMessage
{
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}