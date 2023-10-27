namespace Shared.Application.Agents.Communication.Messages;

public abstract record BaseMessage(Guid Sender, Guid MessageId, IEnumerable<Guid>? Receivers = null, Guid? ResponseTo = null) : IMessage
{
    public string MessageType => GetType().Name;
    public DateTimeOffset CreatedAt { get; protected init; } = DateTimeOffset.UtcNow;
}