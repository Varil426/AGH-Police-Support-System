namespace PatrolService.Application.Orders;

internal abstract record BaseOrder(OrderTypeEnum Type, DateTimeOffset GivenAt);