using System.Collections.Concurrent;
using Simulation.Shared.Communication;

namespace Simulation.Infrastructure.Services;

public interface IMessageSubscriberService
{
    Task<IEnumerable<ISimulationMessage>> GetUnhandledMessages();

    Task ReceiveMessage(ISimulationMessage message);
}