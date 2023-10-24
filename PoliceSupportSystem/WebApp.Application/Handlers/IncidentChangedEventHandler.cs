using Shared.Application.Handlers;
using Shared.Application.Integration.Events;
using WebApp.Application.Services;

namespace WebApp.Application.Handlers;

internal class IncidentChangedEventHandler : IEventHandler<IncidentChangedEvent>
{
    private readonly ICityStateMonitoringService _cityStateMonitoringService;

    public IncidentChangedEventHandler(ICityStateMonitoringService cityStateMonitoringService)
    {
        _cityStateMonitoringService = cityStateMonitoringService;
    }

    public async Task Handle(IncidentChangedEvent @event) => await _cityStateMonitoringService.UpdateIncident(@event.IncidentDto);
}