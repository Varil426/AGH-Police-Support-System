using Shared.Domain.DomainEvents;
using Simulation.Application.Entities.Patrol;
using Action = Simulation.Application.Entities.Patrol.Actions.Action;

namespace Simulation.Application.DomainEvents;

public record PatrolActionChanged(SimulationPatrol Patrol, Action Action) : IDomainEvent;