using System.Runtime.InteropServices.ComTypes;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.CommonTypes.Geo;
using Shared.Domain.Helpers;
using Simulation.Application.Directors.PatrolDirector;
using Simulation.Application.Directors.Settings;
using Simulation.Application.Entities.Patrol;
using Simulation.Application.Services;
using Path = Shared.CommonTypes.Geo.Path;

namespace Simulation.Test;

public class PatrolDirectorMovePatrolTests
{
    private PatrolDirector _patrolDirector = null!;
    private readonly Mock<ILogger<PatrolDirector>> _loggerMock = new();
    private readonly Mock<ISimulationTimeService> _simulationTimeServiceMock = new();
    private readonly Mock<IRouteBuilder> _routeBuilderMock = new();
    private readonly Mock<IMapService> _mapServiceMock = new();
    private readonly PatrolDirectorSettings _patrolDirectorSettings = new(50, 80);
    private readonly Mock<ISimulationPatrol> _patrolMock = new();

    private TimeSpan _timeSinceLastAction = TimeSpan.FromMinutes(15);

    private Position? _newPosition;
    
    [SetUp]
    public void SetUp()
    {
        _patrolDirector = new PatrolDirector(
            _loggerMock.Object,
            _simulationTimeServiceMock.Object,
            _routeBuilderMock.Object,
            _mapServiceMock.Object,
            _patrolDirectorSettings);

        _simulationTimeServiceMock.SetupGet(x => x.SimulationTimeSinceLastAction).Returns(() => _timeSinceLastAction);

        _patrolMock.Setup(x => x.UpdatePosition(It.IsAny<Position>())).Callback<Position>(position => _newPosition = position);
        _patrolMock.SetupGet(x => x.IsInEmergencyState).Returns(false);
    }
    
    [Test]
    public void TestCalculations_Simple_TwoPoints()
    {
        // Arrange
        var start = new Position(50.089356, 19.909992);
        var end = new Position(50.090953, 19.940913);
        var distance = start.GetDistanceTo(end);
        var route = new SimulationPatrolRoute(new List<Path> { new(start, end, distance) });

        _timeSinceLastAction = TimeSpan.FromMinutes(1);
        
        _patrolMock.SetupGet(x => x.Position).Returns(start);
        
        // Act
        _patrolDirector.MovePatrol(_patrolMock.Object, route);
        
        // Assert
        Assert.That(IsInLine(start, end, _newPosition!), Is.True);
    }
    
    [Test]
    public void TestCalculations_MoveWholeRoute()
    {
        // Arrange
        var start = new Position(50.089356, 19.909992);
        var middle = new Position(50.090953, 19.940913);
        var end = new Position(50.080640, 19.942385);
        var route = new SimulationPatrolRoute(new List<Path> { start.Path(middle), middle.Path(end) });
        
        _patrolMock.SetupGet(x => x.Position).Returns(start);
        
        // Act
        _patrolDirector.MovePatrol(_patrolMock.Object, route);
        
        // Assert
        Assert.IsTrue(_newPosition!.Equals(end));
    }

    private static bool IsInLine(Position start, Position end, Position point, double threshold = 1)
    {
        // https://stackoverflow.com/questions/53173712/calculating-distance-of-point-to-linear-line
        var a = start.GetDistanceTo(end);
        var b = start.GetDistanceTo(point);
        var c = end.GetDistanceTo(point);
        var s = (a + b + c) / 2;
        var distance = 2 * Math.Sqrt(s * (s - a) * (s - b) * (s - c)) / a;
        return distance < threshold;
    }
}