namespace Shared.Domain;

public interface IDomainEntity
{
    DateTimeOffset CreatedAt { get; }
    DateTimeOffset UpdatedAt { get; }
}