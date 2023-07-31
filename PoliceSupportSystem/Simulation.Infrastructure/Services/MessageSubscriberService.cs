using Simulation.Shared.Communication;

namespace Simulation.Infrastructure.Services;

internal class MessageSubscriberService : IMessageSubscriberService
{
    private SemaphoreSlim _queueSemaphore = new(1, 1);
    
    private Queue<ISimulationMessage> _messageQueue = new();

    public async Task<IEnumerable<ISimulationMessage>> GetUnhandledMessages()
    {
        await _queueSemaphore.WaitAsync();
        var messages = new List<ISimulationMessage>();
        while (_messageQueue.TryDequeue(out var message))
            messages.Add(message);
        _queueSemaphore.Release();
        return messages;
    }

    public async Task ReceiveMessage(ISimulationMessage message)
    {
        await _queueSemaphore.WaitAsync();
        _messageQueue.Enqueue(message);
        _queueSemaphore.Release();
    }
}