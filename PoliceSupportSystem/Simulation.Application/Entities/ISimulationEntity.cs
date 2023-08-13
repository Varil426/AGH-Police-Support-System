using Shared.Domain.Geo;

namespace Simulation.Application.Entities;

public interface ISimulationEntity
{
    Guid Id { get; }
    
    Position Position { get; }

    IReadOnlyCollection<IService> RelatedServices { get; }

    void AddRelatedService(IService service);

    void RemoveRelatedService(string relatedServiceId);
}