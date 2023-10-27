namespace Shared.Application.Agents.Communication.Messages;

public record IncidentInvestigationStarted(Guid IncidentId, Guid Sender, Guid MessageId, IEnumerable<Guid>? Receivers = null, Guid? ResponseTo = null) : BaseMessage(Sender, MessageId, Receivers, ResponseTo);