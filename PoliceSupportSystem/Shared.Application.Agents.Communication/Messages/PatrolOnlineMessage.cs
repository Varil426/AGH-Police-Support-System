using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Patrol;

namespace Shared.Application.Agents.Communication.Messages;

public record PatrolOnlineMessage(Guid PatrolAgentId, string PatrolId, Position Position, PatrolStatusEnum Status, Guid MessageId, Guid Sender) : BaseMessage(Sender, MessageId);