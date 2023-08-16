using Shared.Domain.DomainEvents;

namespace Shared.Domain;

public abstract class BaseRootDomainEntity : IRootDomainEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyCollection<IDomainEvent> Events => _domainEvents.AsReadOnly();
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; protected set; }
    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}