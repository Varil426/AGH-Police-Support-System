using Simulation.Application.Entities.Incident;

namespace Simulation.Application.Entities.Patrol.Actions;

public record ResolvingIncidentAction(SimulationIncident Incident) : Action;