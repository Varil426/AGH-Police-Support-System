using Microsoft.Extensions.Logging;
using Shared.Application.Agents;
using Shared.Application.Agents.Communication.Messages;
using Shared.Application.Agents.Communication.Signals;
using Shared.Application.Services;

namespace GunService.Application;

public class GunAgent : AgentBase
{
    private readonly IPatrolInfoService _patrolInfoService;
    private readonly ILogger _logger;

    private static readonly IReadOnlyCollection<Type> GunAgentAcceptedMessageTypes = new Type[] {  }
        .AsReadOnly();

    private static readonly IReadOnlyCollection<Type> GunAgentAcceptedSignalTypes = new[] { typeof(GunFiredSignal) }.AsReadOnly();
    
    public GunAgent(IPatrolInfoService patrolInfoService, IMessageService messageService, ILogger logger) : base(
        patrolInfoService.GunAgentId,
        GunAgentAcceptedMessageTypes,
        GunAgentAcceptedSignalTypes,
        messageService,
        logger)
    {
        _patrolInfoService = patrolInfoService;
        _logger = logger;
    }

    protected override Task HandleSignal(IEnvironmentSignal signal) => signal switch
    {
        GunFiredSignal gunFiredSignal => Handle(gunFiredSignal),
        _ => base.HandleSignal(signal)
    };

    private Task Handle(GunFiredSignal gunFiredSignal)
        => MessageService.SendMessageAsync(new GunFiredMessage(Id, Guid.NewGuid(), _patrolInfoService.PatrolAgentId));
}