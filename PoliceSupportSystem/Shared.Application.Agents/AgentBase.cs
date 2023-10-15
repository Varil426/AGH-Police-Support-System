using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;

namespace Shared.Application.Agents;

public abstract class AgentBase : BackgroundService, IAgent
{
    private static readonly IReadOnlyCollection<Type> AgentBaseMessageTypes = new[] { typeof(AcknowledgementMessage) };
    
    private readonly ILogger _logger;
    public Guid Id { get; }
    public IEnumerable<Type> AcceptedMessageTypes { get; }
    public IEnumerable<Type> AcceptedEnvironmentSignalTypes { get; }
    protected IMessageService MessageService { get; }

    private Queue<IMessage> MessageQueue { get; } = new();
    
    private Queue<IEnvironmentSignal> EnvironmentSignalQueue { get; } = new();

    // private readonly SemaphoreSlim _semaphore = new( 2,2);
    private readonly AsyncReaderWriterLock  _readerWriterLock = new();

    private readonly Dictionary<Guid, IMessage?> _awaitingResponse = new();

    private readonly Dictionary<Guid, bool> _awaitingAcknowledge = new();
    
    protected AgentBase(Guid id, IEnumerable<Type> acceptedMessageTypes, IEnumerable<Type> acceptedEnvironmentSignalTypes, IMessageService messageService, ILogger logger)
    {
        var extendedAcceptedMessageTypes = acceptedMessageTypes.ToList();
        extendedAcceptedMessageTypes.AddRange(AgentBaseMessageTypes);
        _logger = logger;
        Id = id;
        AcceptedMessageTypes = extendedAcceptedMessageTypes;
        AcceptedEnvironmentSignalTypes = acceptedEnvironmentSignalTypes;
        MessageService = messageService;
        
        messageService.SubscribeForMessagesAsync(this);
    }

    public async Task PushMessageAsync(IMessage message) => 
        // await PerformSynchronously(() => EnqueueMessage(message), nameof(PushMessageAsync));
        await PerformWriteOperation(() => EnqueueMessage(message));
    
    public async Task PushMessagesAsync(IEnumerable<IMessage> messages) => 
        // await PerformSynchronously(() => messages.ToList().ForEach(EnqueueMessage), nameof(PushMessagesAsync));
        await PerformWriteOperation(() => messages.ToList().ForEach(EnqueueMessage));

    private void EnqueueMessage(IMessage message)
    {
        if (message.ResponseTo.HasValue)
        {
            if (_awaitingResponse.ContainsKey(message.ResponseTo.Value))
                _awaitingResponse[message.ResponseTo.Value] = message;
            if (message is AcknowledgementMessage && _awaitingAcknowledge.ContainsKey(message.ResponseTo.Value))
                _awaitingAcknowledge[message.ResponseTo.Value] = true;
        }
        else
            MessageQueue.Enqueue(message);
    }
    
    public async Task PushEnvironmentSignalAsync(IEnvironmentSignal signal) => 
        // await PerformSynchronously(() => EnvironmentSignalQueue.Enqueue(signal), nameof(PushEnvironmentSignalAsync));
        await PerformWriteOperation(() => EnvironmentSignalQueue.Enqueue(signal));

    public async Task PushEnvironmentSignalsAsync(IEnumerable<IEnvironmentSignal> signals) =>
        // await PerformSynchronously(() => signals.ToList().ForEach(x => EnvironmentSignalQueue.Enqueue(x)), nameof(PushEnvironmentSignalsAsync));
        await PerformWriteOperation(() => signals.ToList().ForEach(x => EnvironmentSignalQueue.Enqueue(x)));
    
    public virtual Task RunAsync(CancellationToken cancellationToken, params string[] args)
    {
        return StartAsync(cancellationToken);
    }

    public Task KillAsync() => StopAsync(new CancellationToken()); // TODO Refactor CancellationToken

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested) // TODO ThrowIfCancellationRequested()
        {
            var messagesToBeHandled = new List<IMessage>();
            var signalsToBeHandled = new List<IEnvironmentSignal>();

            // await PerformSynchronously(
            //     () =>
            //     {
            //         while (MessageQueue.TryDequeue(out var message) && AcceptedMessageTypes.Contains(message.GetType()))
            //             messagesToBeHandled.Add(message);
            //         while (EnvironmentSignalQueue.TryDequeue(out var signal) && AcceptedEnvironmentSignalTypes.Contains(signal.GetType()))
            //             signalsToBeHandled.Add(signal);
            //     }, "CollectMessagesAndSignals");
            
            await PerformReadOperation(
                () =>
                {
                    while (MessageQueue.TryDequeue(out var message) && AcceptedMessageTypes.Contains(message.GetType()))
                        messagesToBeHandled.Add(message);
                    while (EnvironmentSignalQueue.TryDequeue(out var signal) && AcceptedEnvironmentSignalTypes.Contains(signal.GetType()))
                        signalsToBeHandled.Add(signal);
                });

            await Task.WhenAll(messagesToBeHandled.Select(HandleMessage));
            await Task.WhenAll(signalsToBeHandled.Select(HandleSignal));
            await PerformActions();
            
            Thread.Sleep(TimeSpan.FromMilliseconds(50));
        }
    }

    protected virtual Task PerformActions() => Task.CompletedTask;

    protected async Task<TResponseType> Ask<TResponseType>(IMessage message) where TResponseType : class, IMessage // TODO Whole messaging between agents should be reworked
    {
        if (message.Receivers is null || message.Receivers.Count() != 1)
            throw new Exception($"{nameof(Ask)} can only handled messages with exactly 1 receiver.");

        _awaitingResponse[message.MessageId] = null;
        await MessageService.SendMessageAsync(message);

        _logger.LogInformation("Sent \"Ask\" message of type {messageType} with ID: {id}.", message.GetType().Name, message.MessageId);
        IMessage? response = null;
        while (response is null)
        {
            // await PerformSynchronously(() => response = _awaitingResponse[message.MessageId], $"{nameof(Ask)}1");
            await PerformReadOperation(() => response = _awaitingResponse[message.MessageId]);
            await Task.Delay(TimeSpan.FromMicroseconds(50));
        }

        // await PerformSynchronously(() => _awaitingResponse.Remove(message.MessageId), $"{nameof(Ask)}2");
        await PerformWriteOperation(() => _awaitingResponse.Remove(message.MessageId));

        _logger.LogInformation("Received response for message with ID: {id}.", response.ResponseTo);
        return (TResponseType) response;
    }

    protected Task AcknowledgeMessage(IMessageWithAcknowledgeRequired messageWithAcknowledgeRequired) => MessageService.SendMessageAsync(
        new AcknowledgementMessage(Id, Guid.NewGuid(), messageWithAcknowledgeRequired.Sender, messageWithAcknowledgeRequired.MessageId));
    
    protected Task SendWithAcknowledgeRequired(IMessageWithAcknowledgeRequired message) => SendWithAcknowledgeRequired(new List<IMessageWithAcknowledgeRequired> { message });
    
    protected async Task SendWithAcknowledgeRequired(List<IMessageWithAcknowledgeRequired> messages)
    {
        if (messages.Any(x => x.Receivers is null || x.Receivers.Count() != 1))
            throw new Exception($"{nameof(SendWithAcknowledgeRequired)} can only handled messages with exactly 1 receiver.");

        if (!messages.Any())
            return;
        
        _logger.LogInformation("Sending {numberOfMessages} that require acknowledging.", messages.Count);
        foreach (var message in messages)
        {
            _awaitingAcknowledge[message.MessageId] = false;
            await MessageService.SendMessageAsync(message);
        }
        
        var wereAcknowledged = false;
        while (!wereAcknowledged)
        {
            // await PerformSynchronously(() => wereAcknowledged = messages.All(x => _awaitingAcknowledge[x.MessageId]), $"{nameof(SendWithAcknowledgeRequired)}1");
            await PerformReadOperation(() => wereAcknowledged = messages.All(x => _awaitingAcknowledge[x.MessageId]));
            await Task.Delay(TimeSpan.FromMicroseconds(50));
        }
        _logger.LogInformation("All {numberOfMessages} were acknowledged.", messages.Count);

        // await PerformSynchronously(() => messages.ForEach(x => _awaitingAcknowledge.Remove(x.MessageId)), $"{nameof(SendWithAcknowledgeRequired)}2");
        await PerformWriteOperation(() => messages.ForEach(x => _awaitingAcknowledge.Remove(x.MessageId)));
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
            // _semaphore.Dispose();
        }
    }

    protected virtual Task HandleMessage(IMessage message)
    {
        _logger.LogWarning("Cannot handle message of type: {messageType}", message.MessageType);
        return Task.CompletedTask;
    }

    protected virtual Task HandleSignal(IEnvironmentSignal signal)
    {
        _logger.LogWarning("Cannot handle signal of type: {messageType}", signal.Name);
        return Task.CompletedTask;
    }

    // private async Task PerformSynchronously(Action action, string? marker = null) => await PerformSynchronously(
    //     () =>
    // {
    //     action.Invoke();
    //     return Task.CompletedTask;
    // }, marker);
    //
    // private async Task PerformSynchronously(Func<Task> asyncAction, string? marker = null)
    // {
    //     var markerMessage = marker is not null ? $" Marker: {marker}" : string.Empty;
    //     try
    //     {
    //         _logger.LogDebug($"Waiting to enter the semaphore.{marker}");
    //         await _semaphore.WaitAsync();
    //         // _logger.LogDebug($"Entered the semaphore. Marker: {(new System.Diagnostics.StackTrace()).GetFrame((int)frameIndex)?.GetMethod()?.Name ?? "UNKNOWN"}");
    //         _logger.LogDebug($"Entered the semaphore.{markerMessage}");
    //         await asyncAction.Invoke();
    //     }
    //     finally
    //     {
    //         _semaphore.Release();
    //         _logger.LogDebug($"Exited the semaphore.{markerMessage}");
    //     }
    // }

    private async Task PerformWriteOperation(Action action) => await PerformWriteOperation(
        () =>
        {
            action.Invoke();
            return Task.CompletedTask;
        });
    
    private async Task PerformWriteOperation(Func<Task> asyncAction)
    {
        using var _ = await _readerWriterLock.WriterLockAsync();
        await asyncAction.Invoke();
    }
    
    private async Task PerformReadOperation(Action action) => await PerformReadOperation(
        () =>
        {
            action.Invoke();
            return Task.CompletedTask;
        });
    
    private async Task PerformReadOperation(Func<Task> asyncAction)
    {
        using var _ = await _readerWriterLock.ReaderLockAsync();
        await asyncAction.Invoke();
    }
}