namespace PatrolService.Application.Orders;

internal record PatrolOrder(OrderTypeEnum Type, DateTimeOffset GivenAt, string District) : BaseOrder(Type, GivenAt);