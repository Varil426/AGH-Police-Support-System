using Shared.CommonTypes.Geo;

namespace Shared.Domain.Geo;

public record DomainPosition(double Latitude, double Longitude) : Position(Latitude, Longitude), IDomainEntity
{
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; } = DateTimeOffset.UtcNow;
}