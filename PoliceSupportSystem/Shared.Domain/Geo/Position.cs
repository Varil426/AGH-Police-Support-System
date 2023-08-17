namespace Shared.Domain.Geo;

public record Position : IDomainEntity
{
    public Position(double latitude, double longitude)
    {
        this.Latitude = latitude;
        this.Longitude = longitude;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}