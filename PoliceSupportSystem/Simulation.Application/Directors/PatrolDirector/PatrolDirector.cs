using Microsoft.Extensions.Logging;
using Shared.CommonTypes.Patrol;
using Shared.Domain.Helpers;
using Shared.Domain.Patrol;
using Simulation.Application.Directors.Settings;
using Simulation.Application.Entities.Patrol;
using Simulation.Application.Entities.Patrol.Actions;
using Simulation.Application.Entities.Patrol.Orders;
using Simulation.Application.Services;
using Simulation.Communication.Common;
using Simulation.Communication.Messages;

namespace Simulation.Application.Directors.PatrolDirector;

internal class PatrolDirector : IDirector
{
    private const int PositionEqualityThreshold = 1;
    private readonly ILogger<PatrolDirector> _logger;
    private readonly ISimulationTimeService _simulationTimeService;
    private readonly IRouteBuilder _routeBuilder;
    private readonly IMapService _mapService;
    private readonly PatrolDirectorSettings _patrolDirectorSettings;
    private readonly IMessageService _messageService;

    public PatrolDirector(
        ILogger<PatrolDirector> logger,
        ISimulationTimeService simulationTimeService,
        IRouteBuilder routeBuilder,
        IMapService mapService,
        PatrolDirectorSettings patrolDirectorSettings,
        IMessageService messageService)
    {
        _logger = logger;
        _simulationTimeService = simulationTimeService;
        _routeBuilder = routeBuilder;
        _mapService = mapService;
        _patrolDirectorSettings = patrolDirectorSettings;
        _messageService = messageService;
    }

    public async Task Act(ISimulation simulation)
    {
        foreach (var patrol in simulation.Patrols)
        {
            switch (patrol.Action)
            {
                case WaitingAction when patrol is { Status: PatrolStatusEnum.Patrolling, Order: PatrollingOrder patrollingOrder }:
                    var target = await _mapService.GetRandomPositionInDistrict(patrollingOrder.DistrictName) ??
                                 throw new Exception($"Missing random position in a district with name {patrollingOrder.DistrictName}");
                    var route = await _routeBuilder.CreateRoute(patrol.Position, target);
                    patrol.Action = new MovingAction(route);
                    break;
                case MovingAction movingAction:
                    if (movingAction.Route.DestinationReached)
                    {
                        patrol.Action = new WaitingAction();
                        break;
                    }

                    MovePatrol(patrol, movingAction.Route);
                    break;
                case NavigatingAction navigatingAction:
                    if (navigatingAction.Route.DestinationReached)
                    {
                        patrol.Action = new ReadyAction();
                        patrol.IsInEmergencyState = false;
                        var navServices = patrol.GetRelatedServicesOfType(ServiceTypeEnum.NavigationService);
                        await _messageService.SendMessagesAsync(navServices.Select(x => new DestinationReachedMessage(x.Id)));
                        break;
                    }

                    MovePatrol(patrol, navigatingAction.Route);
                    break;
            }
        }
    }

    // Internal visibility for testing!
    internal void MovePatrol(ISimulationPatrol patrol, SimulationPatrolRoute route)
    {
        var distanceTraveledInMeters = _simulationTimeService.SimulationTimeSinceLastAction.TotalSeconds *
                                       (patrol.IsInEmergencyState ? _patrolDirectorSettings.EmergencyPatrolSpeedMetersPerSecond : _patrolDirectorSettings.NormalPatrolSpeedMetersPerSecond);


        var tempPosition = patrol.Position;
        var distanceToNearestTarget = tempPosition.GetDistanceTo(route.CurrentStep.To);

        while (distanceTraveledInMeters > distanceToNearestTarget)
        {
            distanceTraveledInMeters -= distanceToNearestTarget;
            tempPosition = route.CurrentStep.To;
            route.LastStepIndex++;
            if (route.DestinationReached)
            {
                _logger.LogInformation("Patrol: {PatrolId} has reached its destination ({Destination}).", patrol.PatrolId, route.Steps.Last().To);
                break;
            }
            distanceToNearestTarget = tempPosition.GetDistanceTo(route.CurrentStep.To);
        }

        if (route.DestinationReached)
        {
            patrol.UpdatePosition(route.End);
            return;
        }

        var routeLeftToNearestTarget = distanceTraveledInMeters / distanceToNearestTarget;
        var finalPosition = tempPosition + (route.CurrentStep.To - tempPosition) * routeLeftToNearestTarget;
        patrol.UpdatePosition(finalPosition);
    }
}