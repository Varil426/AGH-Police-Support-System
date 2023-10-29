using HqService.Application.Services;
using HqService.Application.Services.DecisionService;
using Microsoft.Extensions.Logging;
using Shared.Application.Agents;
using Shared.Application.Agents.Communication.Messages;
using Shared.Application.Agents.Communication.Messages.PatrolOrders;
using Shared.Application.Agents.Communication.Signals;
using Shared.Application.Factories;
using Shared.Application.Services;
using Shared.CommonTypes.Incident;
using Shared.CommonTypes.Patrol;
using Shared.Domain.Incident;
using Shared.Domain.Patrol;

namespace HqService.Application.Agents;

internal class HqAgent : AgentBase
{
    private readonly IPatrolMonitoringService _patrolMonitoringService;
    private readonly ILogger<HqAgent> _logger;
    private readonly IDomainEventProcessor _domainEventProcessor;
    private readonly IPatrolFactory _patrolFactory;
    private readonly IIncidentMonitoringService _incidentMonitoringService;
    private readonly IDecisionService _decisionService;
    
    private readonly Dictionary<Incident, List<Patrol>> _incidentsToRelatedPatrols = new ();

    private static readonly IReadOnlyCollection<Type> HqAcceptedMessageTypes = new[]
        {
            typeof(PatrolOnlineMessage),
            typeof(PatrolOfflineMessage),
            typeof(CurrentLocationMessage),
            typeof(PatrolStatusChangedMessage),
            typeof(IncidentInvestigationStarted),
            typeof(IncidentResolvedMessage),
            typeof(GunFiredMessage)
        }
        .AsReadOnly();

    private static readonly IReadOnlyCollection<Type> HqAcceptedSignalTypes = new[] { typeof(TestSignal) }.AsReadOnly();

    public HqAgent(
        IHqInfoService hqInfoService,
        IMessageService messageService,
        IPatrolMonitoringService patrolMonitoringService,
        ILogger<HqAgent> logger,
        IDomainEventProcessor domainEventProcessor,
        IPatrolFactory patrolFactory,
        IIncidentMonitoringService incidentMonitoringService,
        IDecisionService decisionService) : base(hqInfoService.HqAgentId, HqAcceptedMessageTypes, HqAcceptedSignalTypes, messageService, logger)
    {
        _patrolMonitoringService = patrolMonitoringService;
        _logger = logger;
        _domainEventProcessor = domainEventProcessor;
        _patrolFactory = patrolFactory;
        _incidentMonitoringService = incidentMonitoringService;
        _decisionService = decisionService;
    }

    protected override async Task PerformActions()
    {
        var onGoingIncidents = await _incidentMonitoringService.GetOnGoingIncidents();
        var orders = await _decisionService.ComputeOrders(onGoingIncidents, _patrolMonitoringService.Patrols);
        var messagesRequiringAcknowledgment = new List<IMessageWithAcknowledgeRequired>();
        foreach (var order in orders)
        {
            switch (order)
            {
                case PatrollingOrder patrollingOrder:
                    messagesRequiringAcknowledgment.Add(new PatrolDistrictOrderMessage(Id, Guid.NewGuid(), patrollingOrder.Id, patrollingOrder.DistrictName));
                    break;
                case HandleIncidentOrder handleIncidentOrder:
                    messagesRequiringAcknowledgment.Add(
                        new HandleIncidentOrderMessage(Id, Guid.NewGuid(), handleIncidentOrder.Id, handleIncidentOrder.IncidentDto.Id, handleIncidentOrder.IncidentDto.Location));
                    var incident = (await _incidentMonitoringService.GetIncidentById(handleIncidentOrder.IncidentDto.Id))!;
                    var patrol = _patrolMonitoringService.Patrols.First(x => x.Id == handleIncidentOrder.Id);
                    if (!_incidentsToRelatedPatrols.ContainsKey(incident))
                        _incidentsToRelatedPatrols.Add(incident, new List<Patrol>());
                    _incidentsToRelatedPatrols[incident].Add(patrol);
                    break;
                default:
                    _logger.LogWarning("Received an unknown order type: {orderType}", order.GetType().Name);
                    break;
            }
        }

        await SendWithAcknowledgeRequired(messagesRequiringAcknowledgment);

        foreach (var messageWithAcknowledgeRequired in messagesRequiringAcknowledgment)
        {
            switch (messageWithAcknowledgeRequired)
            {
                case HandleIncidentOrderMessage handleIncidentOrderMessage:
                    (await _incidentMonitoringService.GetIncidentById(handleIncidentOrderMessage.IncidentId) ?? throw new Exception("Incident not found")).UpdateStatus(
                        IncidentStatusEnum.AwaitingPatrolArrival);
                    break;
            }
        }

        await _domainEventProcessor.ProcessDomainEvents(await _incidentMonitoringService.GetIncidents());
        await _domainEventProcessor.ProcessDomainEvents(_patrolMonitoringService.Patrols);
    }

    protected override Task HandleMessage(IMessage message) =>
        message switch
        {
            PatrolOnlineMessage patrolOnlineMessage => Handle(patrolOnlineMessage),
            PatrolOfflineMessage patrolOfflineMessage => Handle(patrolOfflineMessage),
            CurrentLocationMessage currentLocationMessage => Handle(currentLocationMessage),
            PatrolStatusChangedMessage patrolStatusChangedMessage => Handle(patrolStatusChangedMessage),
            IncidentInvestigationStarted incidentInvestigationStarted => Handle(incidentInvestigationStarted),
            IncidentResolvedMessage incidentResolvedMessage => Handle(incidentResolvedMessage),
            GunFiredMessage gunFiredMessage => Handle(gunFiredMessage),
            _ => base.HandleMessage(message)
        };

    protected override Task HandleSignal(IEnvironmentSignal signal)
    {
        throw new NotImplementedException();
    }

    private async Task Handle(CurrentLocationMessage currentLocationMessage)
    {
        var patrol = _patrolMonitoringService.Patrols.FirstOrDefault(x => x.Id == currentLocationMessage.Sender);
        if (patrol is null)
        {
            _logger.LogWarning($"Received {nameof(currentLocationMessage)} of an unknown sender with id: {currentLocationMessage.Sender}");
            return;
        }
        
        // Fix for handling wrong order of the events - should be improved in the future.
        if (currentLocationMessage.CreatedAt < patrol.UpdatedAt)
            return;
        
        patrol.UpdatePosition(currentLocationMessage.Position);

        await _domainEventProcessor.ProcessDomainEvents(patrol);
    }
    
    private Task Handle(PatrolOnlineMessage patrolOnlineMessage)
    {
        var patrol = _patrolFactory.CreatePatrol(patrolOnlineMessage);
        _patrolMonitoringService.AddPatrol(patrol);
        return _domainEventProcessor.ProcessDomainEvents(patrol);
    }

    private async Task Handle(PatrolStatusChangedMessage patrolStatusChangedMessage)
    {
        var patrol = _patrolMonitoringService.Patrols.FirstOrDefault(x => x.Id == patrolStatusChangedMessage.Sender);
        if (patrol is null)
        {
            _logger.LogWarning($"Received {nameof(PatrolStatusChangedMessage)} for an unknown patrol with ID {patrolStatusChangedMessage.Sender}");
            return;
        }
        
        patrol.UpdateStatus(patrolStatusChangedMessage.Status);
        _logger.LogInformation("A patrol with {ID} changed its status to {NewStatus}", patrol.Id, patrol.Status);
        await _domainEventProcessor.ProcessDomainEvents(patrol);
    }

    private Task Handle(PatrolOfflineMessage patrolOfflineMessage)
    {
        _patrolMonitoringService.RemovePatrol(patrolOfflineMessage.PatrolId);
        // TODO Process domain event - PatrolRemoved
        return Task.CompletedTask;
    }

    private async Task Handle(IncidentInvestigationStarted incidentInvestigationStarted)
    {
        var incident = await _incidentMonitoringService.GetIncidentById(incidentInvestigationStarted.IncidentId) ?? throw new Exception("Incident not found");
        incident.UpdateStatus(IncidentStatusEnum.OnGoingNormal);
        await _domainEventProcessor.ProcessDomainEvents(incident);
    }
    
    private async Task Handle(GunFiredMessage gunFiredMessage)
    {
        var patrol = _patrolMonitoringService.Patrols.First(x => x.Id == gunFiredMessage.Sender);
        var incident = _incidentsToRelatedPatrols.First(x => x.Value.Contains(patrol)).Key;
        
        incident.UpdateStatus(IncidentStatusEnum.OnGoingShooting);
        incident.UpdateType(IncidentTypeEnum.Shooting);
        patrol.UpdateStatus(PatrolStatusEnum.InShooting);
        
        await _domainEventProcessor.ProcessDomainEvents(incident);
        await _domainEventProcessor.ProcessDomainEvents(patrol);
    }
    
    private async Task Handle(IncidentResolvedMessage incidentResolvedMessage)
    {
        var incident = await _incidentMonitoringService.GetIncidentById(incidentResolvedMessage.IncidentId) ?? throw new Exception("Incident not found");
        var patrol = _patrolMonitoringService.Patrols.FirstOrDefault(x => x.Id == incidentResolvedMessage.Sender) ?? throw new Exception("Patrol not found");
        
        incident.UpdateStatus(IncidentStatusEnum.Resolved);
        patrol.UpdateStatus(PatrolStatusEnum.AwaitingOrders);

        await _domainEventProcessor.ProcessDomainEvents(incident);
        await _domainEventProcessor.ProcessDomainEvents(patrol);
    }
}