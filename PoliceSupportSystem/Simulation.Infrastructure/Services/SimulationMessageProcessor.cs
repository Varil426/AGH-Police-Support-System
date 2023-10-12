using Autofac;
using Microsoft.Extensions.Logging;
using Simulation.Application;
using Simulation.Application.Handlers.Messages;
using Simulation.Application.Services;
using Simulation.Communication.Messages;

namespace Simulation.Infrastructure.Services;

internal class SimulationMessageProcessor : ISimulationMessageProcessor
{
    private readonly ILogger<SimulationMessageProcessor> _logger;
    private readonly IComponentContext _ctx;

    private readonly Type _handlerInterface;

    public SimulationMessageProcessor(ILogger<SimulationMessageProcessor> logger, IComponentContext ctx)
    {
        _logger = logger;
        _ctx = ctx;
        _handlerInterface = typeof(ISimulationMessageHandler<>);
    }

    public async Task ProcessAsync(ISimulation simulation, ISimulationMessage simulationMessage)
    {
        var handler = _ctx.Resolve(_handlerInterface.MakeGenericType(simulationMessage.GetType())) as ISimulationMessageHandler ??
                      throw new Exception("Simulation message handler not found");
        _logger.LogInformation($"Started processing a simulation message of type: {simulationMessage.GetType().Name}");
        await handler.HandleAsync(simulation, simulationMessage);
        _logger.LogInformation($"Finished processing a simulation message of type: {simulationMessage.GetType().Name}");
    }

    public async Task ProcessAsync(ISimulation simulation, IEnumerable<ISimulationMessage> simulationMessages)
    {
        foreach (var message in simulationMessages)
            await ProcessAsync(simulation, message);
    }
}