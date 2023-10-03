using Microsoft.Extensions.Logging;
using Simulation.Application.Directors;
using Simulation.Application.Entities;
using Simulation.Application.Services;

namespace Simulation.Application;

internal class Simulation : ISimulation
{
    private readonly ILogger<Simulation> _logger;
    private readonly IMessageService _messageService;
    private readonly ISimulationMessageProcessor _simulationMessageProcessor;
    private readonly IReadOnlyCollection<IDirector> _directors;
    private readonly ISimulationTimeService _simulationTimeService;
    private readonly IDomainEventProcessor _domainEventProcessor;

    private readonly List<IService> _services = new();
    private readonly List<SimulationIncident> _incidents = new();
    private readonly List<SimulationPatrol> _patrols = new();

    public Simulation(
        ILogger<Simulation> logger,
        IMessageService messageService,
        ISimulationMessageProcessor simulationMessageProcessor,
        IReadOnlyCollection<IDirector> directors,
        ISimulationTimeService simulationTimeService,
        IDomainEventProcessor domainEventProcessor)
    {
        _logger = logger;
        _messageService = messageService;
        _simulationMessageProcessor = simulationMessageProcessor;
        _directors = directors;
        _simulationTimeService = simulationTimeService;
        _domainEventProcessor = domainEventProcessor;
    }

    public IReadOnlyCollection<SimulationIncident> Incidents => _incidents.AsReadOnly();
    public IReadOnlyCollection<SimulationPatrol> Patrols => _patrols.AsReadOnly();

    private IReadOnlyCollection<ISimulationRootEntity> SimulationRootEntities => Incidents;

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
            // Perform actions
            await Task.WhenAll(_directors.Select(x => x.Act(this))); // TODO Add Patrol Director
            _simulationTimeService.UpdateLastActionTime();
            // Send updates - Handle Domain Events
            await HandleDomainEvents();
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

    public void AddPatrol(SimulationPatrol patrol)
    {
        if (_patrols.Select(x => x.Id).All(x => x != patrol.Id))
            _patrols.Add(patrol);
        else
            _logger.LogWarning($"Attempted to add a duplicated patrol with ID: {patrol.Id}");
    }

    public void RemoveService(string serviceId)
    {
        var s = _services.FirstOrDefault(x => x.Id.Equals(serviceId, StringComparison.InvariantCultureIgnoreCase));
        if (s is not null)
        {
            RemoveServiceFromRelatedPatrols(s);
            _services.Remove(s);
            RemovePatrolsWithoutRelatedService();
        }
        else
            _logger.LogWarning($"Attempted to remove not existing service with ID: {serviceId}");
    }

    private void RemovePatrolsWithoutRelatedService() => _patrols.Where(x => x.RelatedServices.Count == 0).ToList().ForEach(x => RemovePatrol(x.PatrolId));

    public void RemovePatrol(string patrolId)
    {
        var p = _patrols.FirstOrDefault(x => x.PatrolId.Equals(patrolId, StringComparison.InvariantCultureIgnoreCase));
        if (p is not null)
            _patrols.Remove(p);
        else
            _logger.LogWarning($"Attempted to remove not existing patrol with ID: {patrolId}");
    }

    public void AddIncident(SimulationIncident newIncident)
    {
        if (_incidents.Any(x => x.Id == newIncident.Id))
            throw new Exception("Duplicated incident");

        _incidents.Add(newIncident);
        _logger.LogInformation($"Added a new incident with ID: {newIncident.Id}");
    }
    
    private async Task HandleDomainEvents()
    {
        await _domainEventProcessor.ProcessDomainEvents(SimulationRootEntities/*.SelectMany(x => x.Events)*/);
        // SimulationRootEntities.ToList().ForEach(x => x.ClearDomainEvents());
    }

    private void RemoveServiceFromRelatedPatrols(IService service) => _patrols.Where(x => x.RelatedServices.Contains(service)).ToList().ForEach(x => x.RemoveRelatedService(service));
}