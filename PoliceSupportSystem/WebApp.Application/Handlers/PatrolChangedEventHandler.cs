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

    public Task Handle(PatrolChangedEvent @event)
    {
        var patrol = _cityStateMonitoringService.Patrols.FirstOrDefault(x => x.Id == @event.PatrolDto.Id) ?? throw new Exception("Patrol not found");
        // Fix for handling wrong order of the events - should be improved in the future.
        return @event.CreatedAt < patrol.UpdatedAt ? Task.CompletedTask : _cityStateMonitoringService.UpdatePatrol(@event.PatrolDto);
    }
}