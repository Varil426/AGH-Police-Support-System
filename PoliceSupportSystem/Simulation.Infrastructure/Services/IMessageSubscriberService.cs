using Simulation.Communication.Messages;

namespace Simulation.Infrastructure.Services;

public interface IMessageSubscriberService
{
    Task<IEnumerable<ISimulationMessage>> GetUnhandledMessages();

    Task ReceiveMessage(ISimulationMessage message);
}