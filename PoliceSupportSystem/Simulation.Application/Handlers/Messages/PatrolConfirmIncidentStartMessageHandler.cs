using Microsoft.Extensions.Logging;
using Shared.CommonTypes.Incident;
using Shared.CommonTypes.Patrol;
using Simulation.Application.Entities.Patrol.Actions;
using Simulation.Communication.Messages;

namespace Simulation.Application.Handlers.Messages;

internal class PatrolConfirmIncidentStartMessageHandler : BaseSimulationMessageHandler<PatrolConfirmIncidentStartMessage>
{
    private readonly ILogger<PatrolConfirmIncidentStartMessageHandler> _logger;

    public PatrolConfirmIncidentStartMessageHandler(ILogger<PatrolConfirmIncidentStartMessageHandler> logger)
    {
        _logger = logger;
    }

    public override Task HandleAsync(ISimulation simulation, PatrolConfirmIncidentStartMessage message)
    {
        _logger.LogInformation("Patrol {PatrolId} has started investigating an incident with ID {IncidentId}", message.PatrolId, message.IncidentId);
        var incident = simulation.Incidents.FirstOrDefault(x => x.Id == message.IncidentId) ?? throw new Exception("Incident not found");
        
        var patrol = simulation.Patrols.FirstOrDefault(x => x.PatrolId.Equals(message.PatrolId, StringComparison.InvariantCultureIgnoreCase)) ??
            throw new Exception("Patrol not found");
        if (patrol.Action is not ReadyAction)
        {
            _logger.LogWarning("Patrol {PatrolId} wasn't ready.", patrol.PatrolId);
            return Task.CompletedTask;
        }
        
        incident.UpdateStatus(IncidentStatusEnum.OnGoingNormal);
        patrol.UpdateStatus(PatrolStatusEnum.ResolvingIncident);
        patrol.Action = new ResolvingIncidentAction(simulation.Incidents.First(x => x.Id == message.IncidentId));
        incident.AddRelatedPatrol(patrol);
        
        return Task.CompletedTask;
    }
}