using Shared.Domain.DomainEvents;
using Simulation.Shared.Communication;

namespace Simulation.Application.Services;

internal interface IDomainEventMapper
{
    ISimulationMessage Map(IDomainEvent domainEvent);

    IEnumerable<ISimulationMessage> Map(IEnumerable<IDomainEvent> domainEvents) => domainEvents.Select(Map);
}