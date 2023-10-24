using Shared.CommonTypes.Geo;

namespace Simulation.Application.Entities.Patrol.Orders;

public record MoveOrder(Position Destination) : Order;