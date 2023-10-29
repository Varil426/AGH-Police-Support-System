using GunService.Application;
using Shared.Application.Agents.Communication.Signals;
using Shared.Simulation.Handlers;
using Simulation.Communication.Messages;

namespace GunService.Simulation.Handlers;

internal class GunFiredMessageHandler : ISimulationMessageHandler<GunFiredMessage>
{
    private readonly GunAgent _gunAgent;

    public GunFiredMessageHandler(GunAgent gunAgent)
    {
        _gunAgent = gunAgent;
    }

    public Task Handle(GunFiredMessage simulationMessage) => _gunAgent.PushEnvironmentSignalAsync(new GunFiredSignal());
}