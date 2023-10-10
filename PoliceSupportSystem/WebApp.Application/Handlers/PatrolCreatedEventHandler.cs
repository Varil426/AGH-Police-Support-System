using Microsoft.Extensions.Logging;
using Shared.Application.Handlers;
using Shared.Application.Integration.Events;
using WebApp.Application.Services;

namespace WebApp.Application.Handlers;

internal class PatrolCreatedEventHandler : IEventHandler<PatrolCreatedEvent>
{
    private readonly ILogger<PatrolCreatedEventHandler> _logger;
    private readonly ICityStateMonitoringService _cityStateMonitoringService;

    public PatrolCreatedEventHandler(ILogger<PatrolCreatedEventHandler> logger, ICityStateMonitoringService cityStateMonitoringService)
    {
        _logger = logger;
        _cityStateMonitoringService = cityStateMonitoringService;
    }

    public async Task Handle(PatrolCreatedEvent @event)
    {
        _logger.LogInformation("Received an event info {eventInfo}", @event);
        await _cityStateMonitoringService.AddPatrol(@event.NewPatrolDto);
    }
}