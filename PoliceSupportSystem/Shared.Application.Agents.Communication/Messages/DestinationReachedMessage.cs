namespace Shared.Application.Agents.Communication.Messages;

public record DestinationReachedMessage(Guid Sender, Guid MessageId, Guid Receiver) : BaseMessageWithSingleReceiver(Sender, MessageId, Receiver);