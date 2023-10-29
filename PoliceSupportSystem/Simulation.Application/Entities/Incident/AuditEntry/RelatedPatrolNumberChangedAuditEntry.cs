using Simulation.Application.Entities.Patrol;

namespace Simulation.Application.Entities.Incident.AuditEntry;

public abstract record RelatedPatrolNumberChangedAuditEntry(ISimulationPatrol Patrol, int NewNumberOfPatrols, DateTimeOffset UpdatedAt) : IIncidentAuditEntry;