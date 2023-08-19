using Shared.Domain;
using Shared.Domain.DomainEvents;

namespace Simulation.Application.Services;

internal interface IDomainEventProcessor
{
    Task ProcessDomainEvent(IDomainEvent domainEvent) => ProcessDomainEvents(new [] { domainEvent });

    Task ProcessDomainEvents(IEnumerable<IDomainEvent> domainEvents);

    Task ProcessDomainEvents(IRootDomainEntity rootDomainEntity) => ProcessDomainEvents(rootDomainEntity.Events);

    Task ProcessDomainEvents(IEnumerable<IRootDomainEntity> rootDomainEntities) =>
        ProcessDomainEvents(rootDomainEntities.SelectMany(x => x.Events));
}