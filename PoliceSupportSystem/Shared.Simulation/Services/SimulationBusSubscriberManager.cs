using Autofac.Features.AttributeFilters;
using MessageBus.Core.API;

namespace Shared.Simulation.Services;

internal class SimulationBusSubscriberManager : ISimulationBusSubscriberManager
{
    private readonly IBus _bus;
    private readonly List<IAsyncSubscriber> _subscribers = new();
    private bool _disposed = false;

    public SimulationBusSubscriberManager([KeyFilter(Constants.SimulationBusKey)] IBus bus)
    {
        _bus = bus;
    }


    public IAsyncSubscriber CreateSubscriber(Action<ISubscriberConfigurator> configurator)
    {
        var s = _bus.CreateAsyncSubscriber(configurator);
        _subscribers.Add(s);
        return s;
    }

    ~SimulationBusSubscriberManager() => Dispose(false);
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _subscribers.ForEach(x => x.Dispose());
        }

        _disposed = true;
    }
}