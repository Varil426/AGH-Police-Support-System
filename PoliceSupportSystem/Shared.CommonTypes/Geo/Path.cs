namespace Shared.CommonTypes.Geo;

/// <summary>
/// Represents a simple path between two points.
/// </summary>
/// <param name="From">From.</param>
/// <param name="To">To.</param>
/// <param name="Length">Length in [m].</param>
public record Path(Position From, Position To, double Length);