using Shared.CommonTypes.Geo;
using Shared.Domain.DomainEvents;

namespace Simulation.Application.DomainEvents;

public record PatrolCreated(Guid Id, string PatrolId, Position Position) : IDomainEvent;