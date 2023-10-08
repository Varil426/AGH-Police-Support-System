namespace Shared.Application.Agents.Communication.Messages;

public record BaseMessageWithSingleReceiver : BaseMessage
{
    public BaseMessageWithSingleReceiver(Guid sender, Guid Id, Guid receiver, Guid? responseTo = null) : base(sender, Id, new[] { receiver }, responseTo)
    {
    }
}