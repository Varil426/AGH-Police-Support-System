using Shared.Domain.DomainEvents;
using Simulation.Application.Entities.Patrol;
using Simulation.Application.Entities.Patrol.Orders;

namespace Simulation.Application.DomainEvents;

public record PatrolOrderChanged(SimulationPatrol Patrol, Order? Order) : IDomainEvent;