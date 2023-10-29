using Shared.CommonTypes.Incident;

namespace Simulation.Application.Entities.Incident.AuditEntry;

public record StatusChangedAuditEntry(IncidentStatusEnum? PreviousIncidentStatus, IncidentStatusEnum NewIncidentStatus, DateTimeOffset UpdatedAt) : IIncidentAuditEntry;