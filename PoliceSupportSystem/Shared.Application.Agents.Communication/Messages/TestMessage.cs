namespace Shared.Application.Agents.Communication.Messages;

public record TestMessage/*(Guid Id, Guid? ResponseTo, Guid Sender, IEnumerable<Guid>? Receivers, string MessageType, DateTimeOffset CreatedAt)*/ : IMessage
{
    public Guid Id { get; init; }
    public Guid? ResponseTo { get; init; }
    public Guid Sender { get; init; }
    public IEnumerable<Guid>? Receivers { get; init; }
    public string MessageType { get; init; } = "TEST";
    public DateTimeOffset CreatedAt { get; init; }
}