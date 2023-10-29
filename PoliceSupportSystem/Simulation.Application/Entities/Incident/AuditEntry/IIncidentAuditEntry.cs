namespace Simulation.Application.Entities.Incident.AuditEntry;

public interface IIncidentAuditEntry
{
    DateTimeOffset UpdatedAt { get; }
}