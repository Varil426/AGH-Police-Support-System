﻿using Shared.Domain.DomainEvents;
using Simulation.Application.Entities;
using Simulation.Application.Entities.Patrol;

namespace Simulation.Application.DomainEvents;

public record PatrolRelatedServiceAdded(SimulationPatrol Patrol, IService NewService) : IDomainEvent;