namespace Shared.Application.Agents.Communication.Messages;

public interface IMessage
{
    Guid Id { get; }
    
    Guid? ResponseTo { get; }
    
    Guid Sender { get; }
    
    IEnumerable<Guid>? Receivers { get; }

    string MessageType { get; }

    DateTimeOffset CreatedAt { get; }
}