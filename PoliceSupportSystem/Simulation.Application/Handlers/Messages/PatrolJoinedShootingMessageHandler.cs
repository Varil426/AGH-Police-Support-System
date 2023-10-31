using Microsoft.Extensions.Logging;
using Shared.CommonTypes.Incident;
using Shared.CommonTypes.Patrol;
using Simulation.Application.Entities.Patrol.Actions;
using Simulation.Application.Services;
using Simulation.Communication.Common;
using Simulation.Communication.Messages;

namespace Simulation.Application.Handlers.Messages;

internal class PatrolJoinedShootingMessageHandler : BaseSimulationMessageHandler<PatrolJoinedShootingMessage>
{
    private readonly ILogger<PatrolJoinedShootingMessageHandler> _logger;
    private readonly IMessageService _messageService;

    public PatrolJoinedShootingMessageHandler(ILogger<PatrolJoinedShootingMessageHandler> logger, IMessageService messageService)
    {
        _logger = logger;
        _messageService = messageService;
    }

    public override Task HandleAsync(ISimulation simulation, PatrolJoinedShootingMessage message)
    {
        var incident = simulation.Incidents.FirstOrDefault(x => x.Id == message.IncidentId) ?? throw new Exception("Incident not found");
        var patrol = simulation.Patrols.FirstOrDefault(x => x.PatrolId.Equals(message.PatrolId, StringComparison.InvariantCultureIgnoreCase)) ??
                     throw new Exception("Patrol not found");

        if (incident.Status != IncidentStatusEnum.OnGoingShooting)
        {
            _logger.LogWarning("Incident isn't in a shooting state.");
            
            return _messageService.SendMessagesAsync(patrol.GetRelatedServicesOfType(ServiceTypeEnum.PatrolService).Select(x => new IncidentAlreadyOverMessage(incident.Id, x.Id)));
        }
        
        if (patrol.Action is not ReadyAction)
        {
            _logger.LogWarning("Patrol {PatrolId} wasn't ready.", patrol.PatrolId);
            return Task.CompletedTask;
        }
        
        patrol.UpdateStatus(PatrolStatusEnum.InShooting);
        patrol.Action = new ResolvingIncidentAction(simulation.Incidents.First(x => x.Id == message.IncidentId));
        incident.AddRelatedPatrol(patrol);
        _logger.LogInformation("Patrol {PatrolId} has join a shooting with ID {IncidentId}", message.PatrolId, message.IncidentId);
        
        return Task.CompletedTask;
    }
}