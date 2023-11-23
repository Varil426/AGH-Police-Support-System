using Shared.CommonTypes.Geo;
using Simulation.Communication.Messages;

namespace Simulation.Communication.Queries;

public record GetDistrictQuery(Position Position) : ISimulationQuery<DistrictNameMessage>;