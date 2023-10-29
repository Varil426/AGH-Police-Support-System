using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Incident;
using Simulation.Application.Entities.Incident.AuditEntry;
using Simulation.Application.Entities.Patrol;

namespace Simulation.Application.Entities.Incident;

public class SimulationIncident : Shared.Domain.Incident.Incident, ISimulationRootEntity
{
    private readonly List<IService> _relatedServices = new();
    private readonly List<ISimulationPatrol> _relatedPatrols = new();
    private readonly List<IIncidentAuditEntry> _history = new();

    public Position Position => Location;
    public IReadOnlyCollection<IService> RelatedServices => _relatedServices.AsReadOnly();
    public IReadOnlyCollection<ISimulationPatrol> RelatedPatrols => _relatedPatrols.AsReadOnly();
    public IEnumerable<IIncidentAuditEntry> History => _history.AsReadOnly();
    
    public SimulationIncident(
        Guid id,
        Position location,
        IncidentStatusEnum status,
        IncidentTypeEnum type) : base(id, location, status, type)
    {
        _history.Add(new StatusChangedAuditEntry(null, status, CreatedAt));
    }

    public void AddRelatedService(IService service)
    {
        _relatedServices.Add(service);
        UpdateUpdatedAt();
    }

    public void RemoveRelatedService(string relatedServiceId)
    {
        _relatedServices.Remove(_relatedServices.First(x => x.Id.Equals(relatedServiceId, StringComparison.InvariantCultureIgnoreCase)));
        UpdateUpdatedAt();
    }

    public void AddRelatedPatrol(ISimulationPatrol patrol)
    {
        _relatedPatrols.Add(patrol);
        UpdateUpdatedAt();
        _history.Add(new RelatedPatrolAddedAuditEntry(patrol, _relatedPatrols.Count, UpdatedAt));
    }

    public void RemoveRelatedPatrol(ISimulationPatrol patrol)
    {
        _relatedPatrols.Remove(patrol);
        UpdateUpdatedAt();
        _history.Add(new RelatedPatrolRemovedAuditEntry(patrol, _relatedPatrols.Count, UpdatedAt));
    }

    // public new void ClearDomainEvents() => base.ClearDomainEvents();

    public override void UpdateStatus(IncidentStatusEnum newStatus)
    {
        var previousStatus = Status;
        base.UpdateStatus(newStatus);
        _history.Add(new StatusChangedAuditEntry(previousStatus, newStatus, UpdatedAt));
    }
}