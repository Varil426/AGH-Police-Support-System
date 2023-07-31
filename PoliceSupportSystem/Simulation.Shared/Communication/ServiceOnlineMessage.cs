using Simulation.Shared.Common;

namespace Simulation.Shared.Communication;

public record ServiceOnlineMessage(string ServiceId, ServiceTypeEnum ServiceType) : ISimulationMessage;