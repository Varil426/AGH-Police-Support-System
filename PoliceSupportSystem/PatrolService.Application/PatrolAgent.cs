using Microsoft.Extensions.Logging;
using PatrolService.Application.Orders;
using PatrolService.Application.Services;
using Shared.Application.Agents;
using Shared.Application.Agents.Communication.Messages;
using Shared.Application.Agents.Communication.Messages.PatrolOrders;
using Shared.Application.Agents.Communication.Signals;
using Shared.Application.Services;
using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Patrol;

namespace PatrolService.Application;

public class PatrolAgent : AgentBase
{
    private readonly IPatrolInfoService _patrolInfoService;
    private readonly ILogger<PatrolAgent> _logger;
    private readonly IConfirmationService _confirmationService;

    private static readonly IReadOnlyCollection<Type> PatrolAgentAcceptedMessageTypes = new[]
        { typeof(CurrentLocationMessage), typeof(PatrolDistrictOrderMessage), typeof(HandleIncidentOrderMessage), typeof(DestinationReachedMessage) }.AsReadOnly();

    private static readonly IReadOnlyCollection<Type> PatrolAgentAcceptedEnvironmentSignalTypes = new[] { typeof(IncidentResolvedSignal) }.AsReadOnly();

    private Position? _lastKnowPosition;
    private PatrolStatusEnum _status;
    private BaseOrder? _lastOrder;

    public PatrolAgent(IMessageService messageService, IPatrolInfoService patrolInfoService, ILogger<PatrolAgent> logger, IConfirmationService confirmationService) : base(
        patrolInfoService.PatrolAgentId,
        PatrolAgentAcceptedMessageTypes,
        PatrolAgentAcceptedEnvironmentSignalTypes,
        messageService,
        logger)
    {
        _patrolInfoService = patrolInfoService;
        _logger = logger;
        _confirmationService = confirmationService;
        _status = PatrolStatusEnum.AwaitingOrders;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _lastKnowPosition ??= (await Ask<CurrentLocationMessage>(new AskPositionMessage(Id, Guid.NewGuid(), _patrolInfoService.NavAgentId))).Position;
        await MessageService.SendMessageAsync(new PatrolOnlineMessage(Id, _patrolInfoService.PatrolId, _lastKnowPosition, _status, Guid.NewGuid(), Id));
        await base.ExecuteAsync(stoppingToken);
        await MessageService.SendMessageAsync(new PatrolOfflineMessage(_patrolInfoService.PatrolId, Guid.NewGuid(), Id));
    }

    protected override Task HandleMessage(IMessage message) => message switch
    {
        PatrolDistrictOrderMessage patrolDistrictOrderMessage => Handle(patrolDistrictOrderMessage),
        CurrentLocationMessage currentLocationMessage => Handle(currentLocationMessage),
        HandleIncidentOrderMessage handleIncidentOrderMessage => Handle(handleIncidentOrderMessage),
        DestinationReachedMessage destinationReachedMessage => Handle(destinationReachedMessage),
        _ => base.HandleMessage(message)
    };


    protected override Task HandleSignal(IEnvironmentSignal signal) => signal switch
    {
        IncidentResolvedSignal incidentResolvedSignal => Handle(incidentResolvedSignal),
        _ => base.HandleSignal(signal)
    };

    protected async override Task PerformActions()
    {
        if (_lastOrder is null)
            return;
        
        // _logger.LogInformation($"TEST PatrolOrder {_lastOrder.Type}"); // TODO Remove
    }

    private async Task Handle(CurrentLocationMessage currentLocationMessage)
    {
        if (currentLocationMessage.Receivers is null || !currentLocationMessage.Receivers.Contains(Id) || _lastKnowPosition == currentLocationMessage.Position)
            return;

        _lastKnowPosition = currentLocationMessage.Position;
        await MessageService.SendMessageAsync(new CurrentLocationMessage(_lastKnowPosition, Id, Guid.NewGuid(), DateTimeOffset.UtcNow));
    }
    
    private async Task Handle(PatrolDistrictOrderMessage patrolDistrictOrderMessage)
    {
        if (_lastOrder?.GivenAt > patrolDistrictOrderMessage.CreatedAt)
            return;
        _logger.LogInformation("Patrol: {PatrolId} received a patrol district {DistrictName} order.", _patrolInfoService.PatrolId, patrolDistrictOrderMessage.DistrictName);
        _status = PatrolStatusEnum.Patrolling;
        await MessageService.SendMessageAsync(new PatrolStatusChangedMessage(Id, _status));
        _lastOrder = new PatrolOrder(OrderTypeEnum.Patrol, patrolDistrictOrderMessage.CreatedAt, patrolDistrictOrderMessage.DistrictName);
        await SendWithAcknowledgeRequired(new ShowDistrictMessage(Id, _patrolInfoService.NavAgentId, patrolDistrictOrderMessage.DistrictName));
        await AcknowledgeMessage(patrolDistrictOrderMessage);
    }
    
    private async Task Handle(HandleIncidentOrderMessage handleIncidentOrderMessage)
    {
        if (_lastOrder?.GivenAt > handleIncidentOrderMessage.CreatedAt)
            return;
        _logger.LogInformation("Patrol: {PatrolId} received a handle incident ({IncidentId}) order.", _patrolInfoService.PatrolId, handleIncidentOrderMessage.IncidentId);
        _status = PatrolStatusEnum.ResolvingIncident;
        await MessageService.SendMessageAsync(new PatrolStatusChangedMessage(Id, _status));
        _lastOrder = new HandleIncidentOrder(OrderTypeEnum.Patrol, handleIncidentOrderMessage.CreatedAt, handleIncidentOrderMessage.IncidentLocation, handleIncidentOrderMessage.IncidentId);
        await SendWithAcknowledgeRequired(new NavigateToMessage(Id, _patrolInfoService.NavAgentId, handleIncidentOrderMessage.IncidentLocation));
        await AcknowledgeMessage(handleIncidentOrderMessage);
    }

    private async Task Handle(DestinationReachedMessage destinationReachedMessage)
    {
        switch (_lastOrder)
        {
            case HandleIncidentOrder handleIncidentOrder:
                await _confirmationService.ConfirmIncidentStart(handleIncidentOrder.IncidentId);
                await MessageService.SendMessageAsync(new IncidentInvestigationStarted(handleIncidentOrder.IncidentId, Id, Guid.NewGuid()));
                break;
            // TODO SupportShootingOrder
        }
    }

    private async Task Handle(IncidentResolvedSignal incidentResolvedSignal)
    {
        if (_lastOrder is not HandleIncidentOrder handleIncidentOrder || handleIncidentOrder.IncidentId != incidentResolvedSignal.IncidentId)
            return;

        _lastOrder = null;
        _status = PatrolStatusEnum.AwaitingOrders;
        await MessageService.SendMessageAsync(new IncidentResolvedMessage(Id, Guid.NewGuid(), handleIncidentOrder.IncidentId));
    }
}