namespace Shared.Application.Agents.Communication.Messages;

public record BaseMessageWithAcknowledgeRequired : BaseMessageWithSingleReceiver, IMessageWithAcknowledgeRequired
{
    public BaseMessageWithAcknowledgeRequired(Guid sender, Guid MessageId, Guid receiver, Guid? responseTo = null) : base(sender, MessageId, receiver, responseTo)
    {
    }
}