using Simulation.Communication.Messages;

namespace Simulation.Communication.Queries;

public record GetDistrictsQuery : ISimulationQuery<ListDistrictsMessage>;