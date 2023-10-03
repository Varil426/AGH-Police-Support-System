using Shared.Domain.DomainEvents;
using Simulation.Communication.Messages;

namespace Simulation.Application.Services;

internal interface IDomainEventMapper
{
    ISimulationMessage Map(IDomainEvent domainEvent);

    IEnumerable<ISimulationMessage> Map(IEnumerable<IDomainEvent> domainEvents) => domainEvents.Select(Map);
}