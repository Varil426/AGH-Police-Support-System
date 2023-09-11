using Microsoft.Extensions.Logging;
using Shared.Application.Handlers;
using Shared.Application.Integration.Events;
using WebApp.Application.Services;

namespace WebApp.Application.Handlers;

internal class IncidentCreatedEventHandler : IEventHandler<IncidentCreatedEvent>
{
    private readonly ILogger<IncidentCreatedEventHandler> _logger;
    private readonly ICityStateMonitoringService _cityStateMonitoringService;

    public IncidentCreatedEventHandler(ILogger<IncidentCreatedEventHandler> logger, ICityStateMonitoringService cityStateMonitoringService)
    {
        _logger = logger;
        _cityStateMonitoringService = cityStateMonitoringService;
    }

    public async Task Handle(IncidentCreatedEvent @event)
    {
        _logger.LogInformation("Received an event info {eventInfo}", @event);
        await _cityStateMonitoringService.AddIncident(@event.NewIncidentDto);
    }
}