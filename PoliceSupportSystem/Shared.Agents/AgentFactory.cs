namespace Shared.Agents;

public abstract class AgentFactory
{
    private readonly IMessageService _messageService;

    protected AgentFactory(IMessageService messageService)
    {
        _messageService = messageService;
    }

    protected Task SubscribeAgent(IAgent agent) => _messageService.SubscribeForMessagesAsync(agent);
}