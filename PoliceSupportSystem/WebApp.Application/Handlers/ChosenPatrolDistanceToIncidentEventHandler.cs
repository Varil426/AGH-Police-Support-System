using Shared.Application.Handlers;
using Shared.Application.Integration.Events;
using WebApp.Application.Services.Statistics;

namespace WebApp.Application.Handlers;

internal class ChosenPatrolDistanceToIncidentEventHandler : IEventHandler<ChosenPatrolDistanceToIncidentEvent>
{
    private readonly IStatisticsManager _statisticsManager;

    public ChosenPatrolDistanceToIncidentEventHandler(IStatisticsManager statisticsManager)
    {
        _statisticsManager = statisticsManager;
    }

    public async Task Handle(ChosenPatrolDistanceToIncidentEvent @event) => _statisticsManager.AddDistanceOfChosenPatrolFromIncident(@event.Distance);
}