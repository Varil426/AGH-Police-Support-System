using Shared.Domain.DomainEvents;

namespace Shared.Domain;

public abstract class BaseRootDomainEntity : IRootDomainEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();
    // public IReadOnlyCollection<IDomainEvent> Events => _domainEvents.ToList().AsReadOnly();
    public IReadOnlyCollection<IDomainEvent> Events
    {
        get
        {
            var events = _domainEvents.ToList().AsReadOnly();
            ClearDomainEvents();
            return events;
        }
    }

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; protected set; } = DateTimeOffset.UtcNow;
    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    protected void ClearDomainEvents() => _domainEvents.Clear();
    // public void ClearDomainEvents() => _domainEvents.Clear();
}