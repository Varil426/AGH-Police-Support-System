using Shared.Application.Integration.DTOs;

namespace Shared.Application.Integration.Events;

public record PatrolChangedEvent(PatrolDto PatrolDto, DateTimeOffset CreatedAt) : IEvent;