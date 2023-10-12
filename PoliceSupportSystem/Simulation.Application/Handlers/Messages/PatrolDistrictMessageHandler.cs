using Microsoft.Extensions.Logging;
using Simulation.Application.Entities;
using Simulation.Application.Services;
using Simulation.Communication.Messages;

namespace Simulation.Application.Handlers.Messages;

internal class PatrolDistrictMessageHandler : BaseSimulationMessageHandler<PatrolDistrictMessage>
{
    private readonly ILogger<PatrolDistrictMessageHandler> _logger;
    private readonly IDomainEventProcessor _domainEventProcessor;

    public PatrolDistrictMessageHandler(ILogger<PatrolDistrictMessageHandler> logger, IDomainEventProcessor domainEventProcessor)
    {
        _logger = logger;
        _domainEventProcessor = domainEventProcessor;
    }

    public override async Task HandleAsync(ISimulation simulation, PatrolDistrictMessage message)
    {
        var patrol = simulation.Patrols.FirstOrDefault(x => x.PatrolId.Equals(message.PatrolId, StringComparison.InvariantCultureIgnoreCase)) ??
                     throw new Exception($"Patrol with ID {message.PatrolId} not found");

        patrol.Action = new PatrollingAction(message.DistrictName);
        await _domainEventProcessor.ProcessDomainEvents(patrol);
    }
}