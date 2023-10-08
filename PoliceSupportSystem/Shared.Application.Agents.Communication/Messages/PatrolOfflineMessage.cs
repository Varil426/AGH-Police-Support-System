namespace Shared.Application.Agents.Communication.Messages;

public record PatrolOfflineMessage(string PatrolId, Guid Sender) : BaseMessage(Sender);