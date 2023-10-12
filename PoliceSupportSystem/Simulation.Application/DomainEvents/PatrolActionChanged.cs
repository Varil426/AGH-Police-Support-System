using Shared.Domain.DomainEvents;
using Simulation.Application.Entities;
using Action = Simulation.Application.Entities.Action;

namespace Simulation.Application.DomainEvents;

public record PatrolActionChanged(SimulationPatrol Patrol, Action Action) : IDomainEvent;