namespace Shared.Application.Agents.Communication.Messages;

public abstract record BaseMessageWithAcknowledgeRequired : BaseMessageWithSingleReceiver, IMessageWithAcknowledgeRequired
{
    protected BaseMessageWithAcknowledgeRequired(Guid sender, Guid MessageId, Guid receiver, Guid? responseTo = null) : base(sender, MessageId, receiver, responseTo)
    {
    }
}