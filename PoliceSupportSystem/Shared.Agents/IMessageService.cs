namespace Shared.Agents;

public interface IMessageService
{
    Task SendMessageAsync(IMessage message);

    Task SubscribeForMessagesAsync(IAgent agent); // TODO Internal?
}