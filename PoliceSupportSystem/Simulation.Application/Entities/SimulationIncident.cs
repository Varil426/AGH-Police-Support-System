using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Incident;
using Shared.Domain.Incident;
using Simulation.Application.Entities.Patrol;

namespace Simulation.Application.Entities;

public class SimulationIncident : Incident, ISimulationRootEntity
{
    private readonly List<IService> _relatedServices = new();
    private readonly List<ISimulationPatrol> _relatedPatrols = new();
    private readonly List<IncidentAuditEntry> _history = new();

    public Position Position => Location;
    public IReadOnlyCollection<IService> RelatedServices => _relatedServices.AsReadOnly();
    public IReadOnlyCollection<ISimulationPatrol> RelatedPatrols => _relatedPatrols.AsReadOnly();
    public IEnumerable<IncidentAuditEntry> History => _history.AsReadOnly();
    
    public SimulationIncident(
        Guid id,
        Position location,
        IncidentStatusEnum status,
        IncidentTypeEnum type) : base(id, location, status, type)
    {
        _history.Add(new IncidentAuditEntry(null, status, DateTimeOffset.UtcNow));
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
    }

    public void RemoveRelatedPatrol(ISimulationPatrol patrol)
    {
        _relatedPatrols.Remove(patrol);
        UpdateUpdatedAt();
    }

    // public new void ClearDomainEvents() => base.ClearDomainEvents();

    public override void UpdateStatus(IncidentStatusEnum newStatus)
    {
        var previousStatus = Status;
        base.UpdateStatus(newStatus);
        _history.Add(new IncidentAuditEntry(previousStatus, newStatus, DateTimeOffset.UtcNow));
    }
}