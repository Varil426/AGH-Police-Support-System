namespace Shared.Application.Agents.Communication.Signals;

public interface IEnvironmentSignal
{
    string Name { get; }
    
    string Description { get; }
    
    DateTimeOffset CreatedAt { get; }
}