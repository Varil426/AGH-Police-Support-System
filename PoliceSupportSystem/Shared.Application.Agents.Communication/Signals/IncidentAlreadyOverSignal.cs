namespace Shared.Application.Agents.Communication.Signals;

public record IncidentAlreadyOverSignal(Guid IncidentId) : BaseSignal;