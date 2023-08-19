using Shared.Domain.Geo;
using Shared.Domain.Incident;

namespace Simulation.Application.Entities;

public class SimulationIncident : Incident, ISimulationRootEntity
{
    private readonly List<IService> _relatedServices = new();
    private readonly List<IncidentAuditEntry> _history = new();

    public Position Position => Location;
    public IReadOnlyCollection<IService> RelatedServices => _relatedServices.AsReadOnly();
    public IEnumerable<IncidentAuditEntry> History => _history.AsReadOnly();
    
    public SimulationIncident(
        Guid id,
        Position location,
        IncidentStatusEnum status,
        IncidentTypeEnum type) : base(id, location, status, type)
    {
        _history.Add(new IncidentAuditEntry(null, status, DateTimeOffset.UtcNow));
    }

    public void AddRelatedService(IService service) => _relatedServices.Add(service);

    public void RemoveRelatedService(string relatedServiceId) =>
        _relatedServices.Remove(_relatedServices.First(x => x.Id.Equals(relatedServiceId, StringComparison.InvariantCultureIgnoreCase)));

    public new void ClearDomainEvents() => base.ClearDomainEvents();

    public override void UpdateStatus(IncidentStatusEnum newStatus)
    {
        var previousStatus = Status;
        base.UpdateStatus(newStatus);
        _history.Add(new IncidentAuditEntry(previousStatus, newStatus, DateTimeOffset.UtcNow));
    }
}