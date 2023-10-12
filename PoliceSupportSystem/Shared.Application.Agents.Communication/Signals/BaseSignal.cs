namespace Shared.Application.Agents.Communication.Signals;

public abstract record BaseSignal : IEnvironmentSignal
{
    public string Name => GetType().Name;
    public virtual string Description => string.Empty;
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}