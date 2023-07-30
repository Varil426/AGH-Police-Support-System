using Autofac.Features.AttributeFilters;
using MessageBus.Core.API;
using Shared.Simulation.Settings;
using Simulation.Shared.Communication;

namespace Shared.Simulation.Services;

internal sealed class SimulationMessageBus : ISimulationMessageBus, IDisposable
{
    // private readonly IBus _bus;
    private readonly IRpcAsyncPublisher _publisher;

    public SimulationMessageBus([KeyFilter(Constants.SimulationBusKey)] IBus bus, SimulationCommunicationSettings simulationCommunicationSettings)
    {
        _publisher = bus.CreateAsyncRpcPublisher(
            x =>
            {
                x.SetExchange(simulationCommunicationSettings.SimulationExchangeName);
                x.SetRoutingKey(simulationCommunicationSettings.SimulationQueueName);
            });
    }

    public Task SendSimulationMessage(ISimulationMessage simulationMessage) => _publisher.Send(simulationMessage);

    public void Dispose()
    {
        _publisher.Dispose();
        // _bus.Dispose();
    }
}