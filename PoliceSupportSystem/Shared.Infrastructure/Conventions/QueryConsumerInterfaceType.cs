using MassTransit.Configuration;

namespace Shared.Infrastructure.Conventions;

internal class QueryConsumerInterfaceType : IMessageInterfaceType
{
    readonly Lazy<IMessageConnectorFactory> _consumeConnectorFactory;
    public Type MessageType { get; }

    public QueryConsumerInterfaceType(Type messageType, Type consumerType, Type resultType)
    {
        MessageType = messageType;
        
        _consumeConnectorFactory = new Lazy<IMessageConnectorFactory>(() => (IMessageConnectorFactory)
            Activator.CreateInstance(typeof(ConsumeQueryConnectorFactory<,,>).MakeGenericType(consumerType,
                messageType, resultType))!);
    }

    public IConsumerMessageConnector<T> GetConsumerConnector<T>() where T : class => _consumeConnectorFactory.Value.CreateConsumerConnector<T>();

    public IInstanceMessageConnector<T> GetInstanceConnector<T>() where T : class => _consumeConnectorFactory.Value.CreateInstanceConnector<T>();
}