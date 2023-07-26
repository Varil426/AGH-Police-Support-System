using Simulation.Shared.Communication;

namespace Simulation.Application.Services;

public interface IMessageService
{
    Task<IEnumerable<ISimulationMessage>> GetMessagesAsync();
    Task SendMessageAsync(ISimulationMessage message, string receiver);
    Task SendMessageAsync(IDirectSimulationMessage message);
    Task PublishMessageAsync(ISimulationMessage message);
    Task SendMessagesAsync(IEnumerable<ISimulationMessage> messages, string receiver);
    Task SendMessagesAsync(IEnumerable<IDirectSimulationMessage> messages);
    Task PublishMessagesAsync(IEnumerable<ISimulationMessage> messages);
}