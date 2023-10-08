using Shared.CommonTypes.Geo;

namespace Shared.Application.Agents.Communication.Messages;

public record CurrentLocationMessage(Position Position, Guid Sender, IEnumerable<Guid>? Receivers, Guid? ResponseTo = null) : BaseMessage(Sender, Receivers, ResponseTo);