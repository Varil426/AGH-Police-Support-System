using Simulation.Communication.Messages;

namespace Simulation.Application.Handlers.Messages;

internal class ServiceOfflineMessageHandler : BaseSimulationMessageHandler<ServiceOfflineMessage>
{
    public override Task HandleAsync(ISimulation simulation, ServiceOfflineMessage message)
    {
        simulation.RemoveService(message.ServiceId);
        return Task.CompletedTask;
    }
}