namespace Shared.Application.Agents.Communication.Messages;

public interface IMessageWithAcknowledgeRequired : IMessage
{
    bool IsMessageAcknowledgeRequired => true;
}