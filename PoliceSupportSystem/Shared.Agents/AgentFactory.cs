namespace Shared.Agents;

public abstract class AgentFactory
{
    private readonly IMessageService _messageService;
    private readonly ISignalService _signalService;

    protected AgentFactory(IMessageService messageService, ISignalService signalService)
    {
        _messageService = messageService;
        _signalService = signalService;
    }

    protected async Task SubscribeAgent(IAgent agent)
    {
        await _messageService.SubscribeForMessagesAsync(agent);
        await _signalService.SubscribeForSignalsAsync(agent);
    }
}