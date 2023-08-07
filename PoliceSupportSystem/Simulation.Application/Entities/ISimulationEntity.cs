namespace Simulation.Application.Entities;

public interface ISimulationEntity
{
    Guid Id { get; }
    
    (double, double) Position { get; }
    
    IReadOnlyCollection<IService> RelatedServices { get; }

    void AddRelatedService(IService service);

    void RemoveRelatedService(IService service);
}