using Shared.CommonTypes.Geo;

namespace Shared.Application.Agents.Communication.Messages;

public record CurrentLocationMessage : BaseMessage
{
    public CurrentLocationMessage(Position position, Guid Sender, Guid MessageId, DateTimeOffset createdAt, IEnumerable<Guid>? Receivers = null, Guid? ResponseTo = null) : base(Sender,
        MessageId,
        Receivers,
        ResponseTo)
    {
        Position = position;
        CreatedAt = createdAt;
    }

    public Position Position { get; init; }
}