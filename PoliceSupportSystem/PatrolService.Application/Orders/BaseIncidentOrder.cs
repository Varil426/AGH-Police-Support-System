using Shared.CommonTypes.Geo;

namespace PatrolService.Application.Orders;

internal abstract record BaseIncidentOrder(OrderTypeEnum Type, DateTimeOffset GivenAt, Position IncidentLocation, Guid IncidentId) : BaseOrder(Type, GivenAt);