namespace Shared.Application.Agents.Communication.Signals;

public record IncidentResolvedSignal(Guid IncidentId) : BaseSignal;