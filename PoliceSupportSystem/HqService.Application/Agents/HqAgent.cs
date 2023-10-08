using HqService.Application.Services;
using Microsoft.Extensions.Logging;
using Shared.Application.Agents;
using Shared.Application.Agents.Communication.Messages;
using Shared.Application.Agents.Communication.Signals;
using Shared.Domain.Patrol;

namespace HqService.Application.Agents;

internal class HqAgent : AgentBase
{
    private readonly IPatrolMonitoringService _patrolMonitoringService;
    private static readonly IReadOnlyCollection<Type> HqAcceptedMessageTypes = new[] { typeof(PatrolOnlineMessage), typeof(PatrolOfflineMessage) }.AsReadOnly();
    private static readonly IReadOnlyCollection<Type> HqAcceptedSignalTypes = new[] { typeof(TestSignal) }.AsReadOnly();

    public HqAgent(
        IHqInfoService hqInfoService,
        IMessageService messageService,
        IPatrolMonitoringService patrolMonitoringService,
        Logger<HqAgent> logger) : base(hqInfoService.HqAgentId, HqAcceptedMessageTypes, HqAcceptedSignalTypes, messageService, logger)
    {
        _patrolMonitoringService = patrolMonitoringService;
    }

    protected override Task HandleMessage(IMessage message) =>
        message switch
        {
            PatrolOnlineMessage patrolOnlineMessage => Handle(patrolOnlineMessage),
            PatrolOfflineMessage patrolOfflineMessage => Handle(patrolOfflineMessage),
            _ => base.HandleMessage(message)
        };

    protected override Task HandleSignal(IEnvironmentSignal signal)
    {
        throw new NotImplementedException();
    }

    private Task Handle(PatrolOnlineMessage patrolOnlineMessage)
    {
        _patrolMonitoringService.AddPatrol(new Patrol(Guid.NewGuid(), patrolOnlineMessage.PatrolId, patrolOnlineMessage.Position));
        return Task.CompletedTask;
    }

    private Task Handle(PatrolOfflineMessage patrolOfflineMessage)
    {
        _patrolMonitoringService.RemovePatrol(patrolOfflineMessage.PatrolId);
        return Task.CompletedTask;
    }
}