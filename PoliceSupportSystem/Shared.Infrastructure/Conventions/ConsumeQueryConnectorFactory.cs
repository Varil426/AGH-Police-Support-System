using MassTransit.Configuration;
using Shared.Application;
using Shared.Application.Integration.Queries;

namespace Shared.Infrastructure.Conventions;

internal class ConsumeQueryConnectorFactory<TConsumer, TMessage, TResult> : IMessageConnectorFactory
    where TConsumer : class, IQueryHandler<TMessage, TResult>
    where TMessage : class, IQuery<TResult>
{
    readonly ConsumerMessageConnector<TConsumer, TMessage> _consumerConnector;
    readonly InstanceMessageConnector<TConsumer, TMessage> _instanceConnector;

    public ConsumeQueryConnectorFactory()
    {
        var filter = new MethodQueryConsumerMessageFilter<TConsumer, TMessage, TResult>();

        _consumerConnector = new ConsumerMessageConnector<TConsumer, TMessage>(filter);
        _instanceConnector = new InstanceMessageConnector<TConsumer, TMessage>(filter);
    }

    public IConsumerMessageConnector<T> CreateConsumerConnector<T>() where T : class =>
        _consumerConnector as IConsumerMessageConnector<T> ?? throw new ArgumentException("The consumer type did not match the connector type");

    public IInstanceMessageConnector<T> CreateInstanceConnector<T>() where T : class =>
        _instanceConnector as IInstanceMessageConnector<T> ?? throw new ArgumentException("The consumer type did not match the connector type");
}