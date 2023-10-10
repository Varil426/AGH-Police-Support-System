using HqService.Application.Services;
using Microsoft.Extensions.Logging;
using Shared.Application.Agents;
using Shared.Application.Agents.Communication.Messages;
using Shared.Application.Agents.Communication.Signals;
using Shared.Application.Factories;
using Shared.Application.Services;

namespace HqService.Application.Agents;

internal class HqAgent : AgentBase
{
    private readonly IPatrolMonitoringService _patrolMonitoringService;
    private readonly IDomainEventProcessor _domainEventProcessor;
    private readonly IPatrolFactory _patrolFactory;
    private static readonly IReadOnlyCollection<Type> HqAcceptedMessageTypes = new[] { typeof(PatrolOnlineMessage), typeof(PatrolOfflineMessage) }.AsReadOnly();
    private static readonly IReadOnlyCollection<Type> HqAcceptedSignalTypes = new[] { typeof(TestSignal) }.AsReadOnly();

    public HqAgent(
        IHqInfoService hqInfoService,
        IMessageService messageService,
        IPatrolMonitoringService patrolMonitoringService,
        ILogger<HqAgent> logger,
        IDomainEventProcessor domainEventProcessor,
        IPatrolFactory patrolFactory) : base(hqInfoService.HqAgentId, HqAcceptedMessageTypes, HqAcceptedSignalTypes, messageService, logger)
    {
        _patrolMonitoringService = patrolMonitoringService;
        _domainEventProcessor = domainEventProcessor;
        _patrolFactory = patrolFactory;
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
        var patrol = _patrolFactory.CreatePatrol(patrolOnlineMessage);
        _patrolMonitoringService.AddPatrol(patrol);
        return _domainEventProcessor.ProcessDomainEvents(patrol);
    }

    private Task Handle(PatrolOfflineMessage patrolOfflineMessage)
    {
        _patrolMonitoringService.RemovePatrol(patrolOfflineMessage.PatrolId);
        // TODO Process domain event - PatrolRemoved
        return Task.CompletedTask;
    }
}