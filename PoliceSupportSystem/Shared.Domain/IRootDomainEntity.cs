using Shared.Domain.DomainEvents;

namespace Shared.Domain;

public interface IRootDomainEntity : IDomainEntity
{
    IReadOnlyCollection<IDomainEvent> Events { get; }
}