namespace Shared.Application.Agents.Communication.Messages;

public record AskPositionMessage(Guid Sender, Guid MessageId, Guid Receiver) : BaseMessageWithSingleReceiver(Sender, MessageId, Receiver);