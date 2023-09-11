using MessageBus.Core.API;
using Microsoft.Extensions.Logging;

namespace Shared.Simulation.Services;

internal class SimulationSubscriberErrorService : IErrorSubscriber
{
    private readonly ILogger<SimulationSubscriberErrorService> _logger;

    public SimulationSubscriberErrorService(ILogger<SimulationSubscriberErrorService> logger)
    {
        _logger = logger;
    }

    public void UnhandledException(Exception exception) => _logger.LogError("Unhandled exception: {e}", exception);

    public void MessageDeserializeException(RawBusMessage busMessage, Exception exception) => _logger.LogError("Message deserialization exception: {e}", exception);

    public void MessageDispatchException(RawBusMessage busMessage, Exception exception) => _logger.LogError("Message dispatch exception: {e}", exception);

    public void MessageFilteredOut(RawBusMessage busMessage) => _logger.LogInformation("Message filtered out: {messageId}", busMessage.Name);

    public void UnregisteredMessageArrived(RawBusMessage busMessage) => _logger.LogInformation(
        "Unregistered message: {messageNamespace} {messageType}",
        busMessage.Namespace,
        busMessage.Name);
}