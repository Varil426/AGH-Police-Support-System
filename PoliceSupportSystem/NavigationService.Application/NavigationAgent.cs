using Microsoft.Extensions.Logging;
using NavigationService.Application.Services;
using Shared.Application.Agents;
using Shared.Application.Agents.Communication.Messages;
using Shared.Application.Agents.Communication.Signals;
using Shared.Application.Services;

namespace NavigationService.Application;

public class NavigationAgent : AgentBase
{
    private readonly IPatrolInfoService _patrolInfoService;
    private readonly INavigationService _navigationService;
    private static readonly IEnumerable<Type> NavigationAgentAcceptedMessageTypes = new List<Type> { typeof(AskPositionMessage), typeof(ShowDistrictMessage), typeof(NavigateToMessage) };
    private static readonly IEnumerable<Type> NavigationAgentAcceptedEnvironmentSignalTypes = new [] { typeof(PositionChangedSignal) };

    public NavigationAgent(IMessageService messageService, IPatrolInfoService patrolInfoService, INavigationService navigationService, ILogger<NavigationAgent> logger) : base(
        patrolInfoService.NavAgentId,
        NavigationAgentAcceptedMessageTypes,
        NavigationAgentAcceptedEnvironmentSignalTypes,
        messageService,
        logger)
    {
        _patrolInfoService = patrolInfoService;
        _navigationService = navigationService;
    }

    protected override Task HandleMessage(IMessage message) => message switch
    {
        AskPositionMessage askPositionMessage => Handle(askPositionMessage),
        ShowDistrictMessage showDistrictMessage => Handle(showDistrictMessage),
        NavigateToMessage navigateToMessage => Handle(navigateToMessage),
        _ => base.HandleMessage(message)
    };


    protected override Task HandleSignal(IEnvironmentSignal signal) => signal switch
    {
        PositionChangedSignal positionChangedSignal => Handle(positionChangedSignal),
        _ => base.HandleSignal(signal)
    };

    private async Task Handle(AskPositionMessage askPositionMessage)
    {
        var position = await _navigationService.GetCurrentPosition();
        await MessageService.SendMessageAsync(new CurrentLocationMessage(position, Id, Guid.NewGuid(), new[] { askPositionMessage.Sender }, askPositionMessage.MessageId));
    }
    
    private async Task Handle(ShowDistrictMessage showDistrictMessage)
    {
        await AcknowledgeMessage(showDistrictMessage);
        await _navigationService.DisplayDistrict(showDistrictMessage.DistrictName);
    }
    
    private async Task Handle(NavigateToMessage navigateToMessage)
    {
        await AcknowledgeMessage(navigateToMessage);
        await _navigationService.DisplayRouteTo(navigateToMessage.Position);
    }

    private Task Handle(PositionChangedSignal positionChangedSignal) =>
        MessageService.SendMessageAsync(
            new CurrentLocationMessage(positionChangedSignal.Position, Id, Guid.NewGuid(), new[] { _patrolInfoService.PatrolAgentId }));
}