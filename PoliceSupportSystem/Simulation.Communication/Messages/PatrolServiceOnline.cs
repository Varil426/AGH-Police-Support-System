using Simulation.Communication.Common;

namespace Simulation.Communication.Messages;

public record PatrolServiceOnline(string ServiceId, ServiceTypeEnum ServiceType, string PatrolId) : ServiceOnlineMessage(ServiceId, ServiceType);