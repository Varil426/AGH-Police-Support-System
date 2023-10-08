using Shared.CommonTypes.Geo;
using Shared.Domain.DomainEvents;

namespace Simulation.Application.DomainEvents;

public record PatrolLocationUpdated(Guid PatrolId, Position NewLocation) : IDomainEvent;