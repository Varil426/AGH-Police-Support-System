using Shared.CommonTypes.Geo;
using Shared.Domain;

namespace Simulation.Application.Entities;

public interface ISimulationEntity : IDomainEntity
{
    Guid Id { get; }
    
    Position Position { get; }

    IReadOnlyCollection<IService> RelatedServices { get; }

    void AddRelatedService(IService service);

    void RemoveRelatedService(string relatedServiceId);
}