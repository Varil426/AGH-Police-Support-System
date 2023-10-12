namespace Simulation.Application.Entities;

public abstract record Action
{
    DateTimeOffset PerformingSince = DateTimeOffset.UtcNow;
};