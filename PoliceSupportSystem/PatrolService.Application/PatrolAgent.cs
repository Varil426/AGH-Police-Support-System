using Shared.Application.Agents;
using Shared.Application.Agents.Communication.Messages;
using Shared.Application.Agents.Communication.Signals;
using Shared.Application.Services;

namespace PatrolService.Application;

internal class PatrolAgent : AgentBase
{
    private readonly IStatusService _statusService;
    private static readonly IEnumerable<Type> PatrolAgentAcceptedMessageTypes = Enumerable.Empty<Type>();
    private static readonly IEnumerable<Type> PatrolAgentAcceptedEnvironmentSignalTypes = Enumerable.Empty<Type>();
    
    public PatrolAgent(IMessageService messageService, IStatusService statusService) : base(Guid.NewGuid(), PatrolAgentAcceptedMessageTypes, PatrolAgentAcceptedEnvironmentSignalTypes, messageService)
    {
        _statusService = statusService;
    }

    // protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    // {
    //     await _statusService.AnnounceOnline();
    //     await base.ExecuteAsync(stoppingToken);
    //     await _statusService.AnnounceOffline();
    // }

    protected async override Task HandleMessage(IMessage message)
    {
        switch (message)
        {
            
        }
    }

    protected async override Task HandleSignal(IEnvironmentSignal signal)
    {
        switch (signal)
        {
            
        }
    }
}