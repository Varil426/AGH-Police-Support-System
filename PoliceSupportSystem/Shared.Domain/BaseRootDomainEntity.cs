using Shared.Domain.DomainEvents;

namespace Shared.Domain;

public abstract class BaseRootDomainEntity : IRootDomainEntity
{
    private List<IDomainEvent> _domainEvents = new();
    
    public IReadOnlyCollection<IDomainEvent> Events { get; }

    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}