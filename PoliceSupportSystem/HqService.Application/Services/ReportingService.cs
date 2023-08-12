using HqService.Application.DTOs;
using Microsoft.Extensions.Logging;
using Shared.Domain.Incident;

namespace HqService.Application.Services;

public class ReportingService : IReportingService
{
    private readonly ILogger<ReportingService> _logger;
    private readonly IIncidentMonitoringService _incidentMonitoringService;

    public ReportingService(
        ILogger<ReportingService> logger,
        IIncidentMonitoringService incidentMonitoringService)
    {
        _logger = logger;
        _incidentMonitoringService = incidentMonitoringService;
    }

    public async Task ReportNewIncident(NewIncidentDto newIncidentDto)
    {
        _logger.LogInformation($"Received a new incident info: {newIncidentDto}");
        
        await _incidentMonitoringService.AddIncident(new Incident(newIncidentDto.Id, newIncidentDto.Location, newIncidentDto.Status, newIncidentDto.Type));
        // TODO Notify HQ Agent
    }

    public async Task UpdateStatus(UpdateIncidentDto updateIncidentDto)
    {
        _logger.LogInformation($"Received an incident update info: {updateIncidentDto}");

        await _incidentMonitoringService.UpdatedIncident(updateIncidentDto);
        // TODO Notify HQ Agent
    }
}