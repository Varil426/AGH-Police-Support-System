namespace Shared.Application.Agents;

public interface IMessageService
{
    // Task Handle(IMessage message);
    
    Task SendMessageAsync(IMessage message);

    Task SubscribeForMessagesAsync(IAgent agent); // TODO Internal?
}