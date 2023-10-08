using Shared.CommonTypes.Geo;
using Shared.Domain;
using Simulation.Application.DomainEvents;
using Simulation.Communication.Common;

namespace Simulation.Application.Entities;

public class SimulationPatrol : BaseRootDomainEntity, ISimulationRootEntity
{
    private readonly List<IService> _relatedServices = new();
    public Guid Id { get; }
    public string PatrolId { get; }
    public Position Position { get; private set; }
    public IReadOnlyCollection<IService> RelatedServices => _relatedServices.AsReadOnly();

    public SimulationPatrol(Guid id, string patrolId, Position position)
    {
        Id = id;
        PatrolId = patrolId;
        Position = position;
        AddDomainEvent(new PatrolCreated(Id, PatrolId, Position));
    }

    public void AddRelatedService(IService service)
    {
        _relatedServices.Add(service);
        AddDomainEvent(new PatrolRelatedServiceAdded(this, service));
    }

    public void RemoveRelatedService(IService service) => RemoveRelatedService(service.Id);

    public void RemoveRelatedService(string relatedServiceId) => 
        _relatedServices.Remove(_relatedServices.First(x => x.Id.Equals(relatedServiceId, StringComparison.InvariantCultureIgnoreCase)));

    public IEnumerable<IService> GetRelatedServicesOfType(ServiceTypeEnum serviceType) => RelatedServices.Where(x => x.ServiceType == serviceType);
}