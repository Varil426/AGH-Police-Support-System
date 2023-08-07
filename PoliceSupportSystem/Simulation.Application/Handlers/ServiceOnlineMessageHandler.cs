using Simulation.Shared.Communication;

namespace Simulation.Application.Handlers;

public class ServiceOnlineMessageHandler : BaseSimulationMessageHandler<ServiceOnlineMessage>
{
    public override Task HandleAsync(ISimulation simulation, ServiceOnlineMessage message)
    {

        
        return Task.CompletedTask;
    }
}