using HqService.Application.Services;
using HqService.Application.Services.DecisionService;
using Microsoft.Extensions.Logging;
using Shared.Application.Agents;
using Shared.Application.Agents.Communication.Messages;
using Shared.Application.Agents.Communication.Messages.PatrolOrders;
using Shared.Application.Agents.Communication.Signals;
using Shared.Application.Factories;
using Shared.Application.Services;

namespace HqService.Application.Agents;

internal class HqAgent : AgentBase
{
    private readonly IPatrolMonitoringService _patrolMonitoringService;
    private readonly ILogger<HqAgent> _logger;
    private readonly IDomainEventProcessor _domainEventProcessor;
    private readonly IPatrolFactory _patrolFactory;
    private readonly IIncidentMonitoringService _incidentMonitoringService;
    private readonly IDecisionService _decisionService;
    private static readonly IReadOnlyCollection<Type> HqAcceptedMessageTypes = new[] { typeof(PatrolOnlineMessage), typeof(PatrolOfflineMessage), typeof(CurrentLocationMessage), typeof(PatrolStatusChangedMessage) }.AsReadOnly();
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
        var notHandledIncidents = await _incidentMonitoringService.GetNotHandledIncidents();
        var orders = await _decisionService.ComputeOrders(notHandledIncidents, _patrolMonitoringService.Patrols);
        var messagesRequiringAcknowledgment = new List<IMessageWithAcknowledgeRequired>();
        foreach (var order in orders)
        {
            switch (order)
            {
                case PatrollingOrder patrollingOrder:
                    messagesRequiringAcknowledgment.Add(new PatrolDistrictOrderMessage(Id, Guid.NewGuid(), patrollingOrder.Id, patrollingOrder.DistrictName));
                    break;
                default:
                    _logger.LogWarning("Received an unknown order type: {orderType}", order.GetType().Name);
                    break;
            }
        }

        await SendWithAcknowledgeRequired(messagesRequiringAcknowledgment);
    }

    protected override Task HandleMessage(IMessage message) =>
        message switch
        {
            PatrolOnlineMessage patrolOnlineMessage => Handle(patrolOnlineMessage),
            PatrolOfflineMessage patrolOfflineMessage => Handle(patrolOfflineMessage),
            CurrentLocationMessage currentLocationMessage => Handle(currentLocationMessage),
            PatrolStatusChangedMessage patrolStatusChangedMessage => Handle(patrolStatusChangedMessage),
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
}