namespace Shared.Application.Agents.Communication.Messages;

public record GunFiredMessage(Guid Sender, Guid MessageId, Guid? Receiver = null) : BaseMessage(Sender, MessageId, Receiver.HasValue ? new[] { Receiver.Value } : null);