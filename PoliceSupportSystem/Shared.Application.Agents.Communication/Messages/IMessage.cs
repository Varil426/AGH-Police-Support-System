namespace Shared.Application.Agents.Communication.Messages;

public interface IMessage
{
    Guid MessageId { get; }
    
    Guid? ResponseTo { get; }
    
    Guid Sender { get; }
    
    IEnumerable<Guid>? Receivers { get; }

    string MessageType { get; }

    DateTimeOffset CreatedAt { get; }
}