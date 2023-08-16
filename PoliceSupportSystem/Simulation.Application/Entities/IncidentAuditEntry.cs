using Shared.Domain.Incident;

namespace Simulation.Application.Entities;

public record IncidentAuditEntry(IncidentStatusEnum? PreviousIncidentStatus, IncidentStatusEnum NewIncidentStatus, DateTimeOffset UpdatedAt);