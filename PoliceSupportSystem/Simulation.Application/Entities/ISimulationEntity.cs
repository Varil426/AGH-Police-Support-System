using Shared.CommonTypes.Geo;
using Shared.Domain;

namespace Simulation.Application.Entities;

public interface ISimulationEntity : IDomainEntity
{
    Guid Id { get; }
    
    Position Position { get; }

    public IReadOnlyCollection<IService> RelatedServices { get; }

    public void AddRelatedService(IService service);

    public void RemoveRelatedService(string relatedServiceId);
}