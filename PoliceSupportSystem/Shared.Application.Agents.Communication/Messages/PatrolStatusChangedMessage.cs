using Shared.CommonTypes.Patrol;

namespace Shared.Application.Agents.Communication.Messages;

public record PatrolStatusChangedMessage(Guid Sender, PatrolStatusEnum Status) : BaseMessage(Sender, Guid.NewGuid());