using Simulation.Communication.Messages;

namespace Simulation.Application.Handlers;

public class ServiceOfflineMessageHandler : BaseSimulationMessageHandler<ServiceOfflineMessage>
{
    public override Task HandleAsync(ISimulation simulation, ServiceOfflineMessage message)
    {
        simulation.RemoveService(message.ServiceId);
        return Task.CompletedTask;
    }
}