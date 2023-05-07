namespace Shared.Application.Agents;

public abstract record MessageBase(
    Guid Id,
    Guid? ResponseTo,
    Guid Sender,
    IEnumerable<Guid>? Receivers,
    string MessageType,
    DateTimeOffset CreatedAt) : IMessage
{
    protected MessageBase(Guid sender, Guid? responseTo, IEnumerable<Guid>? receivers, string messageType) : this(
        Guid.NewGuid(),
        responseTo,
        sender,
        receivers,
        messageType,
        DateTimeOffset.UtcNow)
    {
    }
}