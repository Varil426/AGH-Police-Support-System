using System.Reflection;
using MassTransit;
using MassTransit.Configuration;
using Shared.Application;

namespace Shared.Infrastructure.Conventions;

internal class ConsumerMessageConvention<T> : IConsumerMessageConvention
{
    public IEnumerable<IMessageInterfaceType> GetMessageTypes()
    {
        var typeInfo = typeof(T).GetTypeInfo();
        if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
        {
            var interfaceType = new QueryConsumerInterfaceType(typeInfo.GetGenericArguments()[0], typeof(T), typeof(T).GetGenericArguments()[0]);
            if (MessageTypeCache.IsValidMessageType(interfaceType.MessageType))
                yield return interfaceType;
        }

        IEnumerable<QueryConsumerInterfaceType> types = typeof(T).GetInterfaces()
            .Where(x => x.GetTypeInfo().IsGenericType)
            .Where(x => x.GetTypeInfo().GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
            .Select(x => new QueryConsumerInterfaceType(x.GetTypeInfo().GetGenericArguments()[0], typeof(T), x.GetTypeInfo().GetGenericArguments()[1]))
            .Where(x => MessageTypeCache.IsValidMessageType(x.MessageType));

        foreach (var type in types)
            yield return type;
    }
}