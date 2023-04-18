using Microsoft.Extensions.Hosting;

namespace Shared.Agents;

public abstract class AgentBase : BackgroundService, IAgent
{
    public Guid Id { get; }
    public IEnumerable<string> AcceptedMessageTypes { get; }
    public IEnumerable<string> AcceptedEnvironmentSignalTypes { get; }
    protected IMessageService MessageService { get; }

    protected Queue<IMessage> MessageQueue { get; } = new();
    
    protected Queue<IEnvironmentSignal> EnvironmentSignalQueue { get; } = new();

    private readonly SemaphoreSlim _semaphore = new( 1);
    
    protected AgentBase(Guid id, IEnumerable<string> acceptedMessageTypes, IEnumerable<string> acceptedEnvironmentSignalTypes, IMessageService messageService)
    {
        Id = id;
        AcceptedMessageTypes = acceptedMessageTypes;
        AcceptedEnvironmentSignalTypes = acceptedEnvironmentSignalTypes;
        MessageService = messageService;
    }

    public async Task PushMessageAsync(IMessage message)
    {
        await _semaphore.WaitAsync();
        MessageQueue.Enqueue(message);
        _semaphore.Release();
    }

    public async Task PushEnvironmentSignalAsync(IEnvironmentSignal signal)
    {
        await _semaphore.WaitAsync();
        EnvironmentSignalQueue.Enqueue(signal);
        _semaphore.Release();
    }

    public virtual Task RunAsync(CancellationToken cancellationToken, params string[] args)
    {
        return StartAsync(cancellationToken);
    }

    public Task KillAsync() => StopAsync(new CancellationToken()); // TODO Refactor CancellationToken

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested) // TODO ThrowIfCancellationRequested()
        {
            await _semaphore.WaitAsync(stoppingToken);
            
            if (MessageQueue.TryDequeue(out var message) && AcceptedMessageTypes.Contains(message.MessageType))
                await HandleMessage(message);
            
            if (EnvironmentSignalQueue.TryDequeue(out var signal) && AcceptedEnvironmentSignalTypes.Contains(signal.Name))
                await HandleSignal(signal);
                
            
            _semaphore.Release();
            Thread.Sleep(TimeSpan.FromMilliseconds(50));
        }
    }

    public override void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
        base.Dispose();
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            _semaphore.Dispose();
        }
    }
    
    protected abstract Task HandleMessage(IMessage message);
    protected abstract Task HandleSignal(IEnvironmentSignal signal);
}