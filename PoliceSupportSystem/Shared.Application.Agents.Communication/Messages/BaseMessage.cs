namespace Shared.Application.Agents.Communication.Messages;

public record BaseMessage(Guid Sender, IEnumerable<Guid>? Receivers = null, Guid? ResponseTo = null) : IMessage
{
    public Guid Id { get; } = Guid.NewGuid();
    public string MessageType => GetType().Name;
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}