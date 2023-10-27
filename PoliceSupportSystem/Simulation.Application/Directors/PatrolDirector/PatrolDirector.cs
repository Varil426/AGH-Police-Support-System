using Microsoft.Extensions.Logging;
using Shared.CommonTypes.Patrol;
using Shared.Domain.Helpers;
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
            switch (patrol.Status)
            {
                case PatrolStatusEnum.Patrolling:
                    await PerformPatrolling(patrol);
                    break;
                case PatrolStatusEnum.ResolvingIncident:
                case PatrolStatusEnum.InShooting:
                    await PerformIncidentActions(patrol, simulation);
                    break;
                case PatrolStatusEnum.AwaitingOrders:
                    _logger.LogInformation("Patrol {PatrolId} is awaiting orders.", patrol.PatrolId);
                    break;
                default:
                    _logger.LogWarning("Patrol {PatrolId} not handled by the director.", patrol.PatrolId);
                    break;
            }
        }
    }

    private async Task PerformPatrolling(SimulationPatrol patrol)
    {
        var patrollingOrder = patrol.Order as PatrollingOrder ?? throw new Exception();
        switch (patrol.Action)
        {
            case WaitingAction:
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
        }
    }

    private async Task PerformIncidentActions(ISimulationPatrol patrol, ISimulation simulation)
    {
        switch (patrol.Order)
        {
            case MoveOrder moveOrder:
                if (patrol.Action is ResolvingIncidentAction)
                    return;
                if (patrol.Action is MovingAction movingAction && moveOrder.Destination.Equals(movingAction.Route.Steps.Last().To, PositionEqualityThreshold))
                {
                    if (movingAction.Route.DestinationReached)
                    {
                        var navServices = patrol.GetRelatedServicesOfType(ServiceTypeEnum.NavigationService);
                        await _messageService.SendMessagesAsync(navServices.Select(x => new DestinationReachedMessage(x.Id)));
                        patrol.Action = new WaitingAction();
                        return;
                    }
                    
                    MovePatrol(patrol, movingAction.Route);
                    return;
                }
                if (patrol.Position.Equals(moveOrder.Destination, PositionEqualityThreshold))
                    return;
                var route = await _routeBuilder.CreateRoute(patrol.Position, moveOrder.Destination);
                patrol.Action = new MovingAction(route);
                break;
        }
    }

    // Internal visibility for testing!
    internal void MovePatrol(ISimulationPatrol patrol, SimulationPatrolRoute route)
    {
        var distanceTraveledInMeters = _simulationTimeService.SimulationTimeSinceLastAction.TotalSeconds *
                                       (patrol.IsInEmergencyState ? _patrolDirectorSettings.EmergencyPatrolSpeedMetersPerSecond : _patrolDirectorSettings.NormalPatrolSpeedMetersPerSecond);

        // _logger.LogDebug($"Distance traveled {distanceTraveledInMeters} in {_simulationTimeService.SimulationTimeSinceLastAction.TotalSeconds}s"); // TODO Remove
        
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

        _logger.LogDebug($"Temp {tempPosition.Latitude} {tempPosition.Longitude}"); // TODO Remove
        var routeLeftToNearestTarget = distanceTraveledInMeters / distanceToNearestTarget;
        var finalPosition = tempPosition + (route.CurrentStep.To - tempPosition) * routeLeftToNearestTarget;
        patrol.UpdatePosition(finalPosition);
        _logger.LogDebug($"Final {finalPosition.Latitude} {finalPosition.Longitude}"); // TODO Remove
    }
}