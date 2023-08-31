using Shared.Application.Integration.Events;
using Shared.Domain.DomainEvents;

namespace Shared.Application.Services;

public interface IDomainEventMapper
{
    IEvent Map(IDomainEvent @event);
    IEnumerable<IEvent> Map(IEnumerable<IDomainEvent> events);
}