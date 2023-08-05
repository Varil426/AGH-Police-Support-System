using MessageBus.Core.API;

namespace Shared.Infrastructure.Services;

public class BusSubscriberManager : IBusSubscriberManager, IDisposable
{
    private readonly IBus _bus;
    private readonly List<IAsyncSubscriber> _subscribers = new List<IAsyncSubscriber>();
    private bool _disposed = false;

    public BusSubscriberManager(IBus bus)
    {
        _bus = bus;
    }


    public IAsyncSubscriber CreateSubscriber(Action<ISubscriberConfigurator> configurator)
    {
        var s = _bus.CreateAsyncSubscriber(configurator);
        _subscribers.Add(s);
        return s;
    }

    ~BusSubscriberManager() => Dispose(false);
    
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