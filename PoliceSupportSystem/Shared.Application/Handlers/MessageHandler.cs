using Shared.Application.Agents.Communication.Messages;

namespace Shared.Application.Handlers;

/// <summary>
/// Passthrough class for MessageService - Public - Required by Wolverine
/// </summary>
public sealed class MessageHandler : IMessageHandler
{
    private readonly IMessageHandler _messageHandler;

    public MessageHandler(IMessageHandler messageHandler)
    {
        _messageHandler = messageHandler;
    }

    public Task Handle(IMessage message) => _messageHandler.Handle(message);

    public Task Handle(TestMessage message) => Handle((IMessage)message); // TODO For now add those methods for each new message type
}