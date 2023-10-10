using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Shared.Application.Agents;

public abstract class AgentBase : BackgroundService, IAgent
{
    private readonly ILogger _logger;
    public Guid Id { get; }
    public IEnumerable<Type> AcceptedMessageTypes { get; }
    public IEnumerable<Type> AcceptedEnvironmentSignalTypes { get; }
    protected IMessageService MessageService { get; }

    protected Queue<IMessage> MessageQueue { get; } = new();
    
    protected Queue<IEnvironmentSignal> EnvironmentSignalQueue { get; } = new();

    private readonly SemaphoreSlim _semaphore = new( 1,1);

    private readonly Dictionary<Guid, IMessage?> _awaitingResponse = new();
    
    protected AgentBase(Guid id, IEnumerable<Type> acceptedMessageTypes, IEnumerable<Type> acceptedEnvironmentSignalTypes, IMessageService messageService, ILogger logger)
    {
        _logger = logger;
        Id = id;
        AcceptedMessageTypes = acceptedMessageTypes;
        AcceptedEnvironmentSignalTypes = acceptedEnvironmentSignalTypes;
        MessageService = messageService;
        
        messageService.SubscribeForMessagesAsync(this);
    }

    public async Task PushMessageAsync(IMessage message)
    {
        await _semaphore.WaitAsync();
        if (message.ResponseTo is not null && _awaitingResponse.ContainsKey(message.ResponseTo.Value))
            _awaitingResponse[message.ResponseTo.Value] = message;
        else
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
            
            if (MessageQueue.TryDequeue(out var message) && AcceptedMessageTypes.Contains(message.GetType()))
                await HandleMessage(message);
            
            if (EnvironmentSignalQueue.TryDequeue(out var signal) && AcceptedEnvironmentSignalTypes.Contains(signal.GetType()))
                await HandleSignal(signal);
            
            _semaphore.Release();
            Thread.Sleep(TimeSpan.FromMilliseconds(50));
        }
    }

    protected async Task<TResponseType> Ask<TResponseType>(IMessage message) where TResponseType : class, IMessage // TODO Whole messaging between agents should be reworked
    {
        if (message.Receivers is null || message.Receivers.Count() != 1)
            throw new Exception($"{nameof(Ask)} can only handled messages with exactly 1 receiver.");

        _awaitingResponse[message.MessageId] = null;
        await MessageService.SendMessageAsync(message);

        IMessage? response = null;
        while (response is null)
        {
            await _semaphore.WaitAsync();
            response = _awaitingResponse[message.MessageId];
            _semaphore.Release();
        }

        await _semaphore.WaitAsync();
        _awaitingResponse.Remove(message.MessageId);
        _semaphore.Release();

        return (TResponseType) response;
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

    protected virtual Task HandleMessage(IMessage message)
    {
        _logger.LogWarning("Cannot handle message of type: {messageType}", message.MessageType);
        return Task.CompletedTask;
    }
    protected abstract Task HandleSignal(IEnvironmentSignal signal);
}