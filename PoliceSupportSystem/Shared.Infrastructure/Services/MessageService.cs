using Shared.Agents;
using Wolverine;
using IMessage = Shared.Agents.Communication.Messages.IMessage;

namespace Shared.Infrastructure.Services;

internal class MessageService : IMessageService, IDisposable
{
    private readonly IMessageBus _messageBus;
    private readonly ISet<IAgent> _subscribedAgents = new HashSet<IAgent>();
    private readonly SemaphoreSlim _subscribeSemaphore = new SemaphoreSlim(1);

    public MessageService(IMessageBus messageBus)
    {
        _messageBus = messageBus;
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