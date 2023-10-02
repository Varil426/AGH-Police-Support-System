using Simulation.Shared.Common;

namespace Simulation.Shared.Communication;

public record PatrolServiceOnline(string ServiceId, ServiceTypeEnum ServiceType, string PatrolId) : ServiceOnlineMessage(ServiceId, ServiceType);