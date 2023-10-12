using Simulation.Application.Services;
using Simulation.Communication.Messages;

namespace Simulation.Application.Handlers.Messages;

public class ServiceOnlineMessageHandler : BaseSimulationMessageHandler<ServiceOnlineMessage>
{
    private readonly IServiceFactory _serviceFactory;

    public ServiceOnlineMessageHandler(IServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory;
    }

    public override Task HandleAsync(ISimulation simulation, ServiceOnlineMessage message)
    {
        simulation.AddService(_serviceFactory.CreateService(message.ServiceId, message.ServiceType));
        return Task.CompletedTask;
    }
}