using Shared.Domain;
using Shared.Domain.DomainEvents;

namespace Shared.Application.Services;

public interface IDomainEventProcessor
{
    Task ProcessDomainEvent(IDomainEvent domainEvent) => ProcessDomainEvents(new[] { domainEvent });

    Task ProcessDomainEvents(IEnumerable<IDomainEvent> domainEvents);

    Task ProcessDomainEvents(IRootDomainEntity rootDomainEntity)
    {
        var events = rootDomainEntity.Events;
        return ProcessDomainEvents(events);
    }

    Task ProcessDomainEvents(IEnumerable<IRootDomainEntity> rootDomainEntities) =>
        ProcessDomainEvents(rootDomainEntities.SelectMany(x => x.Events));
}