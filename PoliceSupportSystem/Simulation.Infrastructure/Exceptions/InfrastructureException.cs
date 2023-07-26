namespace Simulation.Infrastructure.Exceptions;

public abstract class InfrastructureException : Exception
{
    protected InfrastructureException(string? message) : base(message)
    {
    }
}