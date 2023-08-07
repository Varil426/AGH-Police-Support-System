namespace Shared.Application.Integration.Events;

public interface IEvent
{
    DateTimeOffset CreatedAt { get; }
}