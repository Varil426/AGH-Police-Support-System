namespace Shared.Agents.Communication.Signals;

public record TestSignal(string Name, string Description, DateTimeOffset CreatedAt) : IEnvironmentSignal
{
    
}