using Shared.CommonTypes.Geo;
using Shared.Domain;

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
    }

    public void AddRelatedService(IService service) => _relatedServices.Add(service);

    public void RemoveRelatedService(IService service) => RemoveRelatedService(service.Id); 

    public void RemoveRelatedService(string relatedServiceId) => 
        _relatedServices.Remove(_relatedServices.First(x => x.Id.Equals(relatedServiceId, StringComparison.InvariantCultureIgnoreCase)));
}