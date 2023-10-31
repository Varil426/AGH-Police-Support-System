﻿using Shared.CommonTypes.Geo;

namespace PatrolService.Application.Orders;

internal record SupportShootingOrder(OrderTypeEnum Type, DateTimeOffset GivenAt, Position IncidentLocation, Guid IncidentId) : BaseOrder(Type, GivenAt);