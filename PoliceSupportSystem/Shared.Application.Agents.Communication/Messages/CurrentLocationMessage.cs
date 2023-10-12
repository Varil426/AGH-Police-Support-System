using Shared.CommonTypes.Geo;

namespace Shared.Application.Agents.Communication.Messages;

public record CurrentLocationMessage(Position Position, Guid Sender, Guid MessageId, IEnumerable<Guid>? Receivers = null, Guid? ResponseTo = null) : BaseMessage(Sender, MessageId, Receivers, ResponseTo);