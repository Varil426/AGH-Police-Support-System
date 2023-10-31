using Shared.CommonTypes.Geo;

namespace PatrolService.Application.Orders;

internal record HandleIncidentOrder(OrderTypeEnum Type, DateTimeOffset GivenAt, Position IncidentLocation, Guid IncidentId) : BaseIncidentOrder(
    Type,
    GivenAt,
    IncidentLocation,
    IncidentId);