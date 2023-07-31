using Shared.Application.Agents.Communication.Messages;

namespace Shared.Application.Handlers;

public interface IMessageHandler
{
    public Task Handle(IMessage message);
}