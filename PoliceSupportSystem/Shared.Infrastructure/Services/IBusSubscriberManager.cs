using MessageBus.Core.API;

namespace Shared.Infrastructure.Services;

internal interface IBusSubscriberManager
{
    IAsyncSubscriber CreateSubscriber(Action<ISubscriberConfigurator> configurator);
}