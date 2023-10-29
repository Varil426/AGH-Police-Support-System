using Simulation.Application.Entities.Patrol;

namespace Simulation.Application.Entities.Incident.AuditEntry;

public record RelatedPatrolAddedAuditEntry(ISimulationPatrol Patrol, int NewNumberOfPatrols, DateTimeOffset UpdatedAt) : RelatedPatrolNumberChangedAuditEntry(
    Patrol,
    NewNumberOfPatrols,
    UpdatedAt);