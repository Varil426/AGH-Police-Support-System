using Shared.CommonTypes.Geo;

namespace Shared.Application.Agents.Communication.Messages;

public record PatrolOnlineMessage(string PatrolId, Position Position, Guid MessageId, Guid Sender) : BaseMessage(Sender, MessageId);