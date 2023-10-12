using Shared.Application.Handlers;
using Shared.Application.Integration.Events;
using WebApp.Application.Services;

namespace WebApp.Application.Handlers;

internal class PatrolChangedEventHandler : IEventHandler<PatrolChangedEvent>
{
    private readonly ICityStateMonitoringService _cityStateMonitoringService;

    public PatrolChangedEventHandler(ICityStateMonitoringService cityStateMonitoringService)
    {
        _cityStateMonitoringService = cityStateMonitoringService;
    }

    public Task Handle(PatrolChangedEvent @event) => _cityStateMonitoringService.UpdatePatrol(@event.PatrolDto);
}