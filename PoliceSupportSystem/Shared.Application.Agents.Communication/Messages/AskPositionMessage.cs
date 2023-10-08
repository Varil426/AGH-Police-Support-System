namespace Shared.Application.Agents.Communication.Messages;

public record AskPositionMessage(Guid Sender, Guid Id, Guid Receiver) : BaseMessageWithSingleReceiver(Sender, Id, Receiver);