using HqService.Application.Services;
using Microsoft.Extensions.Logging;
using Shared.Application.Services;
using Shared.Simulation.Handlers;
using Simulation.Communication.Messages;

namespace HqService.Simulation.Handlers;

// internal class IncidentStatusUpdatedHandler : ISimulationMessageHandler<IncidentStatusUpdatedMessage>
// {
//     private readonly IIncidentMonitoringService _incidentMonitoringService;
//     private readonly ILogger<IncidentStatusUpdatedHandler> _logger;
//     private readonly IDomainEventProcessor _domainEventProcessor;
//
//     public IncidentStatusUpdatedHandler(IIncidentMonitoringService incidentMonitoringService, ILogger<IncidentStatusUpdatedHandler> logger, IDomainEventProcessor domainEventProcessor)
//     {
//         _incidentMonitoringService = incidentMonitoringService;
//         _logger = logger;
//         _domainEventProcessor = domainEventProcessor;
//     }
//
//     public async Task Handle(IncidentStatusUpdatedMessage simulationMessage)
//     {
//         var incident = await _incidentMonitoringService.GetIncidentById(simulationMessage.IncidentId) ?? throw new Exception("Incident not found");
//         incident.UpdateStatus(simulationMessage.NewStatus);
//         await _domainEventProcessor.ProcessDomainEvents(incident);
//     }
// }

// TODO Remove