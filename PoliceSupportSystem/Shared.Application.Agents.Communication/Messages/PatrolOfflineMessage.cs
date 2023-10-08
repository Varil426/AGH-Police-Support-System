namespace Shared.Application.Agents.Communication.Messages;

public record PatrolOfflineMessage(string PatrolId, Guid Id, Guid Sender) : BaseMessage(Sender, Id);