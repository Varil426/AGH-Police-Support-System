using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Shared.Agents;

public class MessageService : IMessageService, IDisposable // TODO Maybe some IDisposableGenerator (https://github.com/Elskom/IDisposableGenerator)
{
    // TODO What about a multi-network communication (local network and message broker)? Some message mapper/message resolver?
    private readonly IMessageConverter _messageConverter;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly AsyncEventingBasicConsumer _consumer;

    private readonly HashSet<IAgent> _subscribers = new();
    private readonly SemaphoreSlim _subscribersSemaphore = new(1);

    public MessageService(IMessageConverter messageConverter)
    {
        _messageConverter = messageConverter;
        var factory = new ConnectionFactory { HostName = "localhost" }; // TODO Refactor
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(exclusive: true);

        _consumer = new AsyncEventingBasicConsumer(_channel);
        _consumer.Received += async (_, e) =>
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            await HandleMessageAsync(_messageConverter.Deserialize(message));
        };
    }

    public Task SendMessageAsync(IMessage message)
    {
        var body = Encoding.UTF8.GetBytes(_messageConverter.Serialize(message));
        
        _channel.BasicPublish(exchange: "agent-exchange", // TODO
            routingKey: string.Empty,
            basicProperties: null,
            body: body);

        return Task.CompletedTask;
    }

    public async Task SubscribeForMessagesAsync(IAgent agent)
    {
        await _subscribersSemaphore.WaitAsync();
        _subscribers.Add(agent); // TODO Throw if already present?
        _subscribersSemaphore.Release();
    }

    private async Task HandleMessageAsync(IMessage message)
    {
        await _subscribersSemaphore.WaitAsync();

        var receivers = new List<IAgent>();
        if (message.Receivers != null)
            foreach (var receiver in message.Receivers)
            {
                var sub = _subscribers.FirstOrDefault(
                    x => x.Id == receiver && x.AcceptedMessageTypes.Contains(message.MessageType));
                if (sub is not null)
                    receivers.Add(sub);
            }
        else
            receivers.AddRange(_subscribers.Where(x => x.AcceptedMessageTypes.Contains(message.MessageType)));

        await Task.WhenAll(receivers.Select(x => x.PushMessageAsync(message)));
        
        _subscribersSemaphore.Release();
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
            _connection.Dispose();
            _channel.Dispose();
            _subscribersSemaphore.Dispose();
        }
    }
}