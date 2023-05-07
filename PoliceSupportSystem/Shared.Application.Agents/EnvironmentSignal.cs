namespace Shared.Application.Agents;

public abstract record EnvironmentSignal : IEnvironmentSignal
{
    public string Name { get; }
    public string Description { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    protected EnvironmentSignal(string description) : this(description, DateTimeOffset.UtcNow)
    {
    }

    protected EnvironmentSignal(string description, DateTimeOffset createdAt)
    {
        Description = description;
        CreatedAt = createdAt;
        Name = GetType().Name;
    }
}