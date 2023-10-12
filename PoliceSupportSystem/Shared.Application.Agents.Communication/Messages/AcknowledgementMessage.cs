namespace Shared.Application.Agents.Communication.Messages;

public record AcknowledgementMessage(Guid sender, Guid MessageId, Guid receiver, Guid responseTo) : BaseMessageWithSingleReceiver(sender, MessageId, receiver, responseTo);