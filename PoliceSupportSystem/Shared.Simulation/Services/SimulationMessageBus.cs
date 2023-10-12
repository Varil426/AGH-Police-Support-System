using Autofac.Features.AttributeFilters;
using MessageBus.Core.API;
using Shared.Simulation.Settings;
using Simulation.Communication.Messages;
using Simulation.Communication.Queries;

namespace Shared.Simulation.Services;

internal sealed class SimulationMessageBus : ISimulationMessageBus, IDisposable
{
    // private readonly IBus _bus;
    private readonly IPublisher _publisher;
    private readonly IRpcAsyncPublisher _rpcAsyncPublisher;

    public SimulationMessageBus([KeyFilter(Constants.SimulationBusKey)] IBus bus, SimulationCommunicationSettings simulationCommunicationSettings)
    {
        _publisher = bus.CreatePublisher(
            x =>
            {
                x.SetExchange(simulationCommunicationSettings.SimulationExchangeName);
                x.SetRoutingKey(simulationCommunicationSettings.SimulationQueueName);
            });
        
        _rpcAsyncPublisher = bus.CreateAsyncRpcPublisher(
            x =>
            {
                x.SetExchange(simulationCommunicationSettings.SimulationExchangeName);
                x.SetRoutingKey(simulationCommunicationSettings.SimulationQueueName);
            });
    }

    public Task SendSimulationMessage(ISimulationMessage simulationMessage)
    {
        _publisher.Send(simulationMessage);
        return Task.CompletedTask;
    }

    public async Task<TResultType> QuerySimulationMessage<TQueryType, TResultType>(
        TQueryType query) where TQueryType : ISimulationQuery<TResultType>
        where TResultType : ISimulationMessage =>
        await _rpcAsyncPublisher.Send<TQueryType, TResultType>(query);

    public void Dispose()
    {
        _publisher.Dispose();
        _rpcAsyncPublisher.Dispose();
        // _bus.Dispose();
    }
}