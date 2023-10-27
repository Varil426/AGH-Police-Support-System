namespace Shared.Application.Agents.Communication.Messages;

public record IncidentResolvedMessage(Guid sender, Guid MessageId, Guid IncidentId) : BaseMessage(sender, MessageId);