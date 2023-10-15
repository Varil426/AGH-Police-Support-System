using BAMCIS.GIS;
using Shared.CommonTypes.Geo;

namespace Shared.Domain.Helpers;

public static class GeoHelpers
{
    /// <summary>
    /// Calculates whether two positions are the same.
    /// </summary>
    /// <param name="position1">Position.</param>
    /// <param name="position2">Position.</param>
    /// <param name="threshold">The acceptance threshold in [m].</param>
    public static bool Equals(this Position position1, Position position2, double threshold = 0) =>
        position1.GetDistanceTo(position2) <= threshold;
    
    public static GeoCoordinate ToGeoCoordinate(this Position position) => new(position.Latitude, position.Longitude);

    public static double GetDistanceTo(this Position position1, Position position2, DistanceType type = DistanceType.METERS) =>
        position1.ToGeoCoordinate().DistanceTo(position2.ToGeoCoordinate(), type);

    public static Position Abs(this Position position) => new(Math.Abs(position.Latitude), Math.Abs(position.Longitude));
}