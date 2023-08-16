using Microsoft.Extensions.Logging;
using Simulation.Application.Directors;
using Simulation.Application.Entities;
using Simulation.Application.Services;
using Simulation.Shared.Communication;

namespace Simulation.Application;

internal class Simulation : ISimulation
{
    private readonly ILogger<Simulation> _logger;
    private readonly IMessageService _messageService;
    private readonly ISimulationMessageProcessor _simulationMessageProcessor;
    private readonly IReadOnlyCollection<IDirector> _directors;
    private readonly ISimulationTimeService _simulationTimeService;

    private readonly List<IService> _services = new();
    private readonly List<SimulationIncident> _incidents = new();

    public Simulation(
        ILogger<Simulation> logger,
        IMessageService messageService,
        ISimulationMessageProcessor simulationMessageProcessor,
        IReadOnlyCollection<IDirector> directors,
        ISimulationTimeService simulationTimeService)
    {
        _logger = logger;
        _messageService = messageService;
        _simulationMessageProcessor = simulationMessageProcessor;
        _directors = directors;
        _simulationTimeService = simulationTimeService;
    }

    public IReadOnlyCollection<SimulationIncident> Incidents => _incidents.AsReadOnly();

    public async Task RunAsync(CancellationToken? cancellationToken = null)
    {
        _simulationTimeService.Start();
        _logger.LogInformation("Simulation has started");
        while (cancellationToken is { IsCancellationRequested: false } or null)
        {
            // Receive messages
            var messages = (await _messageService.GetMessagesAsync()).OrderBy(x => x.CreatedAt);
            // Process messages - Update state
            await _simulationMessageProcessor.ProcessAsync(this, messages);
            // TODO Perform actions
            await Task.WhenAll(_directors.Select(x => x.Act(this)));
            _simulationTimeService.UpdateLastActionTime();
            // TODO Send updates - Handle Domain Events
            await Task.Delay(30);
        }
    }

    public void AddService(IService service)
    {
        if (!_services.Select(x => x.Id).Any(x => x.Equals(service.Id, StringComparison.InvariantCultureIgnoreCase)))
            _services.Add(service);
        else
            _logger.LogWarning($"Attempted to add a duplicated service with ID: {service.Id}");
    }

    public void RemoveService(string serviceId)
    {
        var s = _services.FirstOrDefault(x => x.Id.Equals(serviceId, StringComparison.InvariantCultureIgnoreCase));
        if (s is not null)
            _services.Remove(s);
        else
            _logger.LogWarning($"Attempted to remove not existing service with ID: {serviceId}");
    }

    public async Task AddIncident(SimulationIncident newIncident)
    {
        if (_incidents.Any(x => x.Id == newIncident.Id))
            throw new Exception("Duplicated incident");

        _incidents.Add(newIncident);
        await _messageService.PublishMessageAsync(new NewIncidentMessage(newIncident.Id, newIncident.Position, newIncident.Type, newIncident.Status));
        _logger.LogInformation($"Added a new incident with ID: {newIncident.Id}");
    }
}