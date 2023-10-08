using Microsoft.Extensions.Logging;
using Shared.Application.Agents;
using Shared.Application.Agents.Communication.Messages;
using Shared.Application.Agents.Communication.Signals;
using Shared.Application.Services;
using Shared.CommonTypes.Geo;

namespace PatrolService.Application;

internal class PatrolAgent : AgentBase
{
    private readonly IPatrolInfoService _patrolInfoService;
    private static readonly IEnumerable<Type> PatrolAgentAcceptedMessageTypes = new[] { typeof(CurrentLocationMessage) };
    private static readonly IEnumerable<Type> PatrolAgentAcceptedEnvironmentSignalTypes = Enumerable.Empty<Type>();

    private Position? _lastKnowPosition;

    public PatrolAgent(IMessageService messageService, IPatrolInfoService patrolInfoService, ILogger<PatrolAgent> logger) : base(
        patrolInfoService.PatrolAgentId,
        PatrolAgentAcceptedMessageTypes,
        PatrolAgentAcceptedEnvironmentSignalTypes,
        messageService,
        logger)
    {
        _patrolInfoService = patrolInfoService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _lastKnowPosition ??= (await Ask<CurrentLocationMessage>(new AskPositionMessage(Id, Guid.NewGuid(), _patrolInfoService.NavAgentId))).Position;
        await MessageService.SendMessageAsync(new PatrolOnlineMessage(_patrolInfoService.PatrolId, _lastKnowPosition, Guid.NewGuid(), Id));
        await base.ExecuteAsync(stoppingToken);
        await MessageService.SendMessageAsync(new PatrolOfflineMessage(_patrolInfoService.PatrolId, Guid.NewGuid(), Id));
    }

    protected override Task HandleMessage(IMessage message) => message switch
    {
        _ => base.HandleMessage(message)
    };


    protected async override Task HandleSignal(IEnvironmentSignal signal)
    {
        switch (signal)
        {
        }
    }
}