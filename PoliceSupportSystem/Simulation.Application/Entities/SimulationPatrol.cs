using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Patrol;
using Shared.Domain.Patrol;
using Simulation.Application.DomainEvents;
using Simulation.Communication.Common;

namespace Simulation.Application.Entities;

public class SimulationPatrol : Patrol, ISimulationRootEntity
{
    private readonly List<IService> _relatedServices = new();
    public IReadOnlyCollection<IService> RelatedServices => _relatedServices.AsReadOnly();

    public SimulationPatrol(Guid id, string patrolId, Position position, PatrolStatusEnum status) : base(id, patrolId, position, status)
    {
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