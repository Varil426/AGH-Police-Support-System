using Shared.Application;
using Shared.Application.Agents;
using IMessage = Shared.Application.Agents.Communication.Messages.IMessage;

namespace Shared.Infrastructure.Services;

internal class MessageService : IMessageService, IMessageHandler, IDisposable
{
    private readonly IMessageBus _messageBus;
    private readonly ISet<IAgent> _subscribedAgents = new HashSet<IAgent>();
    private readonly SemaphoreSlim _subscribeSemaphore = new SemaphoreSlim(1);

    public MessageService(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }
    
    public Task Handle(IMessage message)
    {
        // TODO Validate receiver
        var validReceivers = _subscribedAgents.Where(x => x.AcceptedMessageTypes.Any(type => type.IsInstanceOfType(message)));
        return Task.WhenAll(validReceivers.Select(x => x.PushMessageAsync(message)));
    }

    public async Task SendMessageAsync(IMessage message) => await _messageBus.SendAsync(message);

    public async Task SubscribeForMessagesAsync(IAgent agent)
    {
        await _subscribeSemaphore.WaitAsync();
        _subscribedAgents.Add(agent);
        _subscribeSemaphore.Release();
    }
    
    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            _subscribeSemaphore.Dispose();
        }
    }
}