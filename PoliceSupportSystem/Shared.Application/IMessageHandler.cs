using Shared.Application.Agents.Communication.Messages;

namespace Shared.Application;

public interface IMessageHandler
{
    public Task Handle(IMessage message);
}