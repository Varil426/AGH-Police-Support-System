namespace WebApp.Application.Services.Statistics;

public class StateHistory<TState>
{
    public IList<(TState state, DateTimeOffset since)> States { get; } = new List<(TState state, DateTimeOffset since)>();
}