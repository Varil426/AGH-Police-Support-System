using Shared.CommonTypes.Geo;

namespace Shared.Application.Agents.Communication.Signals;

public record PositionChangedSignal(Position Position) : BaseSignal;