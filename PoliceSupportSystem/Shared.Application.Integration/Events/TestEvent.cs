namespace Shared.Application.Integration.Events;

public record TestEvent(string Value, DateTimeOffset CreatedAt) : IEvent; // TODO Remove