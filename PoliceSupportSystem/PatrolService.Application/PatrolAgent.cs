using Microsoft.Extensions.Logging;
using PatrolService.Application.Orders;
using Shared.Application.Agents;
using Shared.Application.Agents.Communication.Messages;
using Shared.Application.Agents.Communication.Messages.PatrolOrders;
using Shared.Application.Agents.Communication.Signals;
using Shared.Application.Services;
using Shared.CommonTypes.Geo;
using Shared.CommonTypes.Patrol;

namespace PatrolService.Application;

internal class PatrolAgent : AgentBase
{
    private readonly IPatrolInfoService _patrolInfoService;
    private readonly ILogger<PatrolAgent> _logger;
    private static readonly IEnumerable<Type> PatrolAgentAcceptedMessageTypes = new[] { typeof(CurrentLocationMessage), typeof(PatrolDistrictOrderMessage) };
    private static readonly IEnumerable<Type> PatrolAgentAcceptedEnvironmentSignalTypes = Enumerable.Empty<Type>();

    private Position? _lastKnowPosition;
    private PatrolStatusEnum _status;
    private BaseOrder? _lastOrder;

    public PatrolAgent(IMessageService messageService, IPatrolInfoService patrolInfoService, ILogger<PatrolAgent> logger) : base(
        patrolInfoService.PatrolAgentId,
        PatrolAgentAcceptedMessageTypes,
        PatrolAgentAcceptedEnvironmentSignalTypes,
        messageService,
        logger)
    {
        _patrolInfoService = patrolInfoService;
        _logger = logger;
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
        _ => base.HandleMessage(message)
    };


    protected async override Task HandleSignal(IEnvironmentSignal signal)
    {
        switch (signal)
        {
        }
    }

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
        await MessageService.SendMessageAsync(new CurrentLocationMessage(_lastKnowPosition, Id, Guid.NewGuid()));
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
}