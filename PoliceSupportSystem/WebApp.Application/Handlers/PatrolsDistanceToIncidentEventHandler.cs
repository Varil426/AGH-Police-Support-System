using Shared.Application.Handlers;
using Shared.Application.Integration.Events;
using WebApp.Application.Services.Statistics;

namespace WebApp.Application.Handlers;

internal class PatrolsDistanceToIncidentEventHandler : IEventHandler<PatrolsDistanceToIncidentEvent>
{
    private readonly IStatisticsManager _statisticsManager;

    public PatrolsDistanceToIncidentEventHandler(IStatisticsManager statisticsManager)
    {
        _statisticsManager = statisticsManager;
    }

    public async Task Handle(PatrolsDistanceToIncidentEvent @event) => @event.Distances.ToList().ForEach(x => _statisticsManager.AddDistanceOfConsideredPatrolFromIncident(x));
}