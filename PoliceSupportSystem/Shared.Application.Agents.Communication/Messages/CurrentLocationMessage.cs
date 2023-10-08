using Shared.CommonTypes.Geo;

namespace Shared.Application.Agents.Communication.Messages;

public record CurrentLocationMessage(Position Position, Guid Sender, Guid Id, IEnumerable<Guid>? Receivers, Guid? ResponseTo = null) : BaseMessage(Sender, Id, Receivers, ResponseTo);