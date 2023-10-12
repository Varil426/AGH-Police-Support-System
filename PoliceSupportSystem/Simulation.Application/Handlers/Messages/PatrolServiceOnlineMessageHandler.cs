using Simulation.Application.Services;
using Simulation.Communication.Messages;

namespace Simulation.Application.Handlers.Messages;

internal class PatrolServiceOnlineMessageHandler : BaseSimulationMessageHandler<PatrolServiceOnline>
{
    private readonly IServiceFactory _serviceFactory;
    private readonly ISimulationPatrolFactory _patrolFactory;

    public PatrolServiceOnlineMessageHandler(IServiceFactory serviceFactory, ISimulationPatrolFactory patrolFactory)
    {
        _serviceFactory = serviceFactory;
        _patrolFactory = patrolFactory;
    }
    
    public override Task HandleAsync(ISimulation simulation, PatrolServiceOnline message)
    {
        var service = _serviceFactory.CreateService(message.ServiceId, message.ServiceType);
        simulation.AddService(service);

        var patrol = simulation.Patrols.FirstOrDefault(x => x.PatrolId.Equals(message.PatrolId, StringComparison.InvariantCultureIgnoreCase));
        if (patrol is null)
        {
            patrol =_patrolFactory.CreatePatrol(message.PatrolId);
            simulation.AddPatrol(patrol);
        }
        patrol.AddRelatedService(service);
        
        return Task.CompletedTask;
    }
}