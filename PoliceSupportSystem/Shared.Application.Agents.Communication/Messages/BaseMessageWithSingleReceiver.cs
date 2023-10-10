namespace Shared.Application.Agents.Communication.Messages;

public abstract record BaseMessageWithSingleReceiver : BaseMessage
{
    public BaseMessageWithSingleReceiver(Guid sender, Guid MessageId, Guid receiver, Guid? responseTo = null) : base(sender, MessageId, new[] { receiver }, responseTo)
    {
    }
}