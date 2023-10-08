namespace Shared.Application.Agents.Communication.Messages;

public record AskPositionMessage(Guid Sender, Guid Receiver) : BaseMessageWithSingleReceiver(Sender, Receiver);