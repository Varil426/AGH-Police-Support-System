namespace Shared.Application.Agents.Communication.Messages;

public record PatrolOfflineMessage(string PatrolId, Guid MessageId, Guid Sender) : BaseMessage(Sender, MessageId);