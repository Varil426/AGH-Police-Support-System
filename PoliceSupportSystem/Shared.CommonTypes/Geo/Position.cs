using Reinforced.Typings.Attributes;

namespace Shared.CommonTypes.Geo;

[TsInterface]
public record Position(double Latitude, double Longitude)
{
    public static Position operator +(Position a, Position b) => new(a.Latitude + b.Latitude, a.Longitude + b.Longitude);
    public static Position operator -(Position a, Position b) => a+-b;
    public static Position operator -(Position a) => new(-a.Latitude, -a.Longitude);

    public static Position operator *(Position a, double scale) => new(a.Latitude * scale, a.Longitude * scale);
}