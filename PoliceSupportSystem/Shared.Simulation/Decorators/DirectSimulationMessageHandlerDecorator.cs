using Microsoft.Extensions.Logging;
using Shared.Application.Services;
using Shared.Simulation.Handlers;
using Simulation.Communication.Messages;

namespace Shared.Simulation.Decorators;

[Decorator]
internal class DirectSimulationMessageHandlerDecorator<TSimulationMessageType> : ISimulationMessageHandler<TSimulationMessageType> where TSimulationMessageType : ISimulationMessage
{
    private readonly ISimulationMessageHandler<TSimulationMessageType> _decorated;
    private readonly ILogger<DirectSimulationMessageHandlerDecorator<TSimulationMessageType>> _logger;
    private readonly IServiceInfoService _serviceInfoService;

    public DirectSimulationMessageHandlerDecorator(
        ISimulationMessageHandler<TSimulationMessageType> decorated,
        ILogger<DirectSimulationMessageHandlerDecorator<TSimulationMessageType>> logger,
        IServiceInfoService serviceInfoService)
    {
        _decorated = decorated;
        _logger = logger;
        _serviceInfoService = serviceInfoService;
    }

    public Task Handle(TSimulationMessageType simulationMessage)
    {
        if (simulationMessage is not IDirectSimulationMessage directSimulationMessage ||
            directSimulationMessage.Receiver.Equals(_serviceInfoService.Id, StringComparison.InvariantCulture)) 
            return _decorated.Handle(simulationMessage);
        
        _logger.LogWarning($"Service: {_serviceInfoService.Id} received simulation message for ${directSimulationMessage.Receiver}. Skipping.");
        return Task.CompletedTask;

    }
}