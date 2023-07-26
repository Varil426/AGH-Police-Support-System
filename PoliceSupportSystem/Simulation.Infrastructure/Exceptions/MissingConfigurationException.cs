namespace Simulation.Infrastructure.Exceptions;

public class MissingConfigurationException : InfrastructureException
{
    public MissingConfigurationException(string configurationName) : base($"{configurationName}: is missing.")
    {
    }
}