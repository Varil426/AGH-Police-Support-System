using MessageBus.Core.API;

namespace Shared.Simulation.Services;

internal interface ISimulationBusSubscriberManager
{
    IAsyncSubscriber CreateSubscriber(Action<ISubscriberConfigurator> configurator);
}