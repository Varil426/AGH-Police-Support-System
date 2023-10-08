namespace Shared.Application.Agents.Communication.Messages;

public record BaseMessageWithSingleReceiver : BaseMessage
{
    public BaseMessageWithSingleReceiver(Guid sender, Guid receiver, Guid? responseTo = null) : base(sender, new[] { receiver }, responseTo)
    {
    }
}