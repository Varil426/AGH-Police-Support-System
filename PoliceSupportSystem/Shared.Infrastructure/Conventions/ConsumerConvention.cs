using MassTransit.Configuration;

namespace Shared.Infrastructure.Conventions;

internal class ConsumerConvention : IConsumerConvention
{
    public IConsumerMessageConvention GetConsumerMessageConvention<T>() where T : class
    {
        return new ConsumerMessageConvention<T>();
    }
}