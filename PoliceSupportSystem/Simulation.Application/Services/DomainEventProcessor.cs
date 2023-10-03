using Shared.Domain.DomainEvents;
using Simulation.Communication.Messages;

namespace Simulation.Application.Services;

internal class DomainEventProcessor : IDomainEventProcessor
{
    private readonly IDomainEventMapper _mapper;
    private readonly IMessageService _messageService;

    public DomainEventProcessor(IDomainEventMapper mapper, IMessageService messageService)
    {
        _mapper = mapper;
        _messageService = messageService;
    }

    public async Task ProcessDomainEvents(IEnumerable<IDomainEvent> domainEvents)
    {
        var messages = _mapper.Map(domainEvents).ToList();
        var directMessages = messages.OfType<IDirectSimulationMessage>();
        var notDirectMessages = messages.Where(x => !directMessages.Contains(x));
        await _messageService.SendMessagesAsync(directMessages);
        await _messageService.PublishMessagesAsync(notDirectMessages);
    }
}