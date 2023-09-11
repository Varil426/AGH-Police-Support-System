using Microsoft.Extensions.Logging;
using Shared.Application.Factories;
using Shared.Application.Integration.DTOs;
using Shared.Application.Services;

namespace HqService.Application.Services;

public class ReportingService : IReportingService
{
    private readonly ILogger<ReportingService> _logger;
    private readonly IIncidentMonitoringService _incidentMonitoringService;
    private readonly IDomainEventProcessor _domainEventProcessor;
    private readonly IIncidentFactory _incidentFactory;

    public ReportingService(
        ILogger<ReportingService> logger,
        IIncidentMonitoringService incidentMonitoringService,
        IDomainEventProcessor domainEventProcessor,
        IIncidentFactory incidentFactory)
    {
        _logger = logger;
        _incidentMonitoringService = incidentMonitoringService;
        _domainEventProcessor = domainEventProcessor;
        _incidentFactory = incidentFactory;
    }

    public async Task ReportNewIncident(NewIncidentDto newIncidentDto)
    {
        _logger.LogInformation($"Received a new incident info: {newIncidentDto}");

        var newIncident = _incidentFactory.CreateIncident(newIncidentDto);
        await _incidentMonitoringService.AddIncident(newIncident);
        // TODO Notify HQ Agent

        await _domainEventProcessor.ProcessDomainEvents(newIncident);
    }

    public async Task UpdateStatus(UpdateIncidentDto updateIncidentDto)
    {
        _logger.LogInformation($"Received an incident update info: {updateIncidentDto}");

        await _incidentMonitoringService.UpdatedIncident(updateIncidentDto);
        // TODO Notify HQ Agent
        var incident = (await _incidentMonitoringService.GetIncidentById(updateIncidentDto.Id))!;
        await _domainEventProcessor.ProcessDomainEvents(incident);
    }
}