using NavigationService.Application.Services;
using Shared.Application.Agents;
using Shared.Application.Agents.Communication.Messages;
using Shared.Application.Agents.Communication.Signals;
using Shared.Application.Services;

namespace NavigationService.Application;

public class NavigationAgent  : AgentBase
{
    private readonly IPatrolInfoService _patrolInfoService;
    private readonly INavigationService _navigationService;
    private static readonly IEnumerable<Type> NavigationAgentAcceptedMessageTypes = new List<Type> { typeof(AskPositionMessage) };
    private static readonly IEnumerable<Type> NavigationAgentAcceptedEnvironmentSignalTypes = Enumerable.Empty<Type>();

    public NavigationAgent(IMessageService messageService, IPatrolInfoService patrolInfoService, INavigationService navigationService) : base(
        patrolInfoService.NavAgentId,
        NavigationAgentAcceptedMessageTypes,
        NavigationAgentAcceptedEnvironmentSignalTypes,
        messageService)
    {
        _patrolInfoService = patrolInfoService;
        _navigationService = navigationService;
    }

    protected override async Task HandleMessage(IMessage message)
    {
        switch (message)
        {
            case AskPositionMessage askPositionMessage:
                await Handle(askPositionMessage);
                break;
        }
    }

    protected async override Task HandleSignal(IEnvironmentSignal signal)
    {
        switch (signal)
        {
            
        }
    }

    private async Task Handle(AskPositionMessage askPositionMessage)
    {
        var position = await _navigationService.GetCurrentPosition();
        await MessageService.SendMessageAsync(new CurrentLocationMessage(position, Id, new[] { askPositionMessage.Sender }, askPositionMessage.Id));
    }
}