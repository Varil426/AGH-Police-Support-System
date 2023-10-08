using Shared.CommonTypes.Geo;

namespace Shared.Application.Agents.Communication.Messages;

public record PatrolOnlineMessage(string PatrolId, Position Position, Guid Sender) : BaseMessage(Sender);