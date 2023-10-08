using Shared.Domain.DomainEvents;
using Simulation.Application.Entities;

namespace Simulation.Application.DomainEvents;

public record PatrolRelatedServiceAdded(SimulationPatrol Patrol, IService NewService) : IDomainEvent;